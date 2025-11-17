using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BibAnalyzer.Utils
{
    public static class DoiUtils
    {
        /// <summary>
        /// Asynchronously retrieves the page count for a publication identified by its DOI using the Crossref API.
        /// </summary>
        /// <remarks>The method attempts to extract page information from several possible fields in the
        /// Crossref metadata. If the page range cannot be determined, the method returns null. Supplying a custom
        /// HttpClient allows for connection reuse and custom configuration.</remarks>
        /// <param name="doi">The Digital Object Identifier (DOI) of the publication for which to retrieve the page count. Cannot be null,
        /// empty, or consist only of white-space characters.</param>
        /// <param name="client">An optional HttpClient instance to use for the request. If null, a new HttpClient is created and disposed
        /// internally.</param>
        /// <returns>A nullable integer representing the number of pages in the publication if available; otherwise, null if the
        /// page information cannot be determined.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the doi parameter is null, empty, or consists only of white-space characters.</exception>
        public static async Task<int?> GetPageCountAsync(string doi, HttpClient client = null)
        {
            if (string.IsNullOrWhiteSpace(doi)) throw new ArgumentNullException(nameof(doi));
            bool disposeClient = false;
            if (client == null)
            {
                client = new HttpClient();
                disposeClient = true;
            }

            try
            {
                // Recomenda-se fornecer um User-Agent com contato (Crossref etiquette)
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0 (mailto:seu-email@exemplo.com)");

                string url = $"https://api.crossref.org/works/{Uri.EscapeDataString(doi)}";
                using var resp = await client.GetAsync(url).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode) return null;

                using var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var doc = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);

                if (!doc.RootElement.TryGetProperty("message", out var msg)) return null;

                // Tenta várias propriedades que podem conter informação de páginas
                string? pages = TryGetString(msg, "page") ?? TryGetString(msg, "pages");
                if (string.IsNullOrWhiteSpace(pages))
                {
                    // Alguns registros têm page-first / page-last
                    var first = TryGetString(msg, "page-first");
                    var last = TryGetString(msg, "page-last");
                    if (!string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(last))
                    {
                        pages = $"{first}-{last}";
                    }
                }

                if (string.IsNullOrWhiteSpace(pages)) return null;

                // Normalizar e extrair números (captura todos os inteiros presentes)
                var nums = Regex.Matches(pages, @"\d+")
                                .Select(m => int.Parse(m.Value))
                                .ToArray();
                if (nums.Length == 0) return null;
                if (nums.Length == 1) return 1; // somente uma numeração -> tratamos como 1 página (ou indeterminado)

                int min = nums.Min();
                int max = nums.Max();
                if (max < min) (min, max) = (max, min);

                return Math.Abs(max - min) + 1;
            }
            finally
            {
                if (disposeClient) client?.Dispose();
            }

            static string TryGetString(JsonElement el, string prop)
            {
                if (el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String)
                    return v.GetString();
                return null;
            }
        }

        /// <summary>
        /// Result for PDF fetch: bytes and best-effort filename.
        /// </summary>
        public sealed record PdfFetchResult(byte[] Bytes, string FileName);

        /// <summary>
        /// Attempts to fetch a PDF for the given DOI and returns bytes plus a suggested file name.
        /// Strategies (in order):
        /// 1) Content negotiation against https://doi.org/{doi} with Accept: application/pdf
        /// 2) Use Crossref metadata "message.link" entries that point to PDF (and extract title for filename)
        /// 3) Use Unpaywall (requires email) to find an OA PDF
        /// Returns PdfFetchResult on success, otherwise null.
        /// </summary>
        public static async Task<PdfFetchResult> FetchPdfAsync(
            string doi,
            HttpClient client = null,
            string unpaywallEmail = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(doi)) throw new ArgumentNullException(nameof(doi));
            bool disposeClient = false;
            if (client == null)
            {
                client = new HttpClient();
                disposeClient = true;
            }

            try
            {
                if (!client.DefaultRequestHeaders.UserAgent.Any())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("BibAnalyzer/1.0 (mailto:your-email@example.com)");
                }

                string? crossrefTitle = null;

                // Try Crossref early to possibly use title as filename fallback
                try
                {
                    var crossrefUrl = $"https://api.crossref.org/works/{Uri.EscapeDataString(doi)}";
                    using var crResp = await client.GetAsync(crossrefUrl, cancellationToken).ConfigureAwait(false);
                    if (crResp.IsSuccessStatusCode)
                    {
                        using var stream = await crResp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                        if (doc.RootElement.TryGetProperty("message", out var msg))
                        {
                            // title may be an array
                            if (msg.TryGetProperty("title", out var titleEl))
                            {
                                if (titleEl.ValueKind == JsonValueKind.Array && titleEl.GetArrayLength() > 0 && titleEl[0].ValueKind == JsonValueKind.String)
                                    crossrefTitle = titleEl[0].GetString();
                                else if (titleEl.ValueKind == JsonValueKind.String)
                                    crossrefTitle = titleEl.GetString();
                            }
                        }
                    }
                }
                catch
                {
                    // ignore metadata errors; we'll still try other strategies
                }

                // 1) Content negotiation with doi.org
                var doiResolver = new Uri($"https://doi.org/{Uri.EscapeDataString(doi)}");
                using (var req = new HttpRequestMessage(HttpMethod.Get, doiResolver))
                {
                    req.Headers.Accept.Clear();
                    req.Headers.Accept.ParseAdd("application/pdf");
                    using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    if (resp.IsSuccessStatusCode)
                    {
                        if (IsPdfResponse(resp.Content.Headers))
                        {
                            var bytes = await resp.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                            var name = GetFileNameFromResponse(resp, doi, crossrefTitle);
                            return new PdfFetchResult(bytes, name);
                        }

                        var finalUri = resp.RequestMessage?.RequestUri;
                        if (finalUri != null && finalUri != doiResolver)
                        {
                            var res = await TryDownloadIfPdfUrlWithNameAsync(client, finalUri, doi, crossrefTitle, cancellationToken).ConfigureAwait(false);
                            if (res != null) return res;
                        }
                    }
                }

                // 2) Try Crossref metadata links
                try
                {
                    var crossrefUrl = $"https://api.crossref.org/works/{Uri.EscapeDataString(doi)}";
                    using (var crResp = await client.GetAsync(crossrefUrl, cancellationToken).ConfigureAwait(false))
                    {
                        if (crResp.IsSuccessStatusCode)
                        {
                            using var stream = await crResp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                            if (doc.RootElement.TryGetProperty("message", out var msg) &&
                                msg.TryGetProperty("link", out var links) &&
                                links.ValueKind == JsonValueKind.Array)
                            {
                                // try explicit pdf links first
                                foreach (var link in links.EnumerateArray())
                                {
                                    if (link.TryGetProperty("URL", out var urlEl) && urlEl.ValueKind == JsonValueKind.String)
                                    {
                                        var url = urlEl.GetString()!;
                                        var contentType = link.TryGetProperty("content-type", out var ct) && ct.ValueKind == JsonValueKind.String
                                            ? ct.GetString()
                                            : null;
                                        if (!string.IsNullOrWhiteSpace(contentType) && contentType!.Contains("pdf", StringComparison.OrdinalIgnoreCase))
                                        {
                                            var res = await TryDownloadUrlAsPdfWithNameAsync(client, url, doi, crossrefTitle, cancellationToken).ConfigureAwait(false);
                                            if (res != null) return res;
                                        }
                                    }
                                }

                                // best-effort try other links
                                foreach (var link in links.EnumerateArray())
                                {
                                    if (link.TryGetProperty("URL", out var urlEl) && urlEl.ValueKind == JsonValueKind.String)
                                    {
                                        var url = urlEl.GetString()!;
                                        var res = await TryDownloadUrlAsPdfWithNameAsync(client, url, doi, crossrefTitle, cancellationToken).ConfigureAwait(false);
                                        if (res != null) return res;
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // ignore crossref link parsing errors
                }

                // 3) Try Unpaywall (requires email param). This often finds OA PDFs.
                if (!string.IsNullOrWhiteSpace(unpaywallEmail))
                {
                    try
                    {
                        var upUrl = $"https://api.unpaywall.org/v2/{Uri.EscapeDataString(doi)}?email={Uri.EscapeDataString(unpaywallEmail)}";
                        using var upResp = await client.GetAsync(upUrl, cancellationToken).ConfigureAwait(false);
                        if (upResp.IsSuccessStatusCode)
                        {
                            using var stream = await upResp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                            if (doc.RootElement.TryGetProperty("best_oa_location", out var best) && best.ValueKind == JsonValueKind.Object)
                            {
                                if (TryGetString(best, "url_for_pdf") is string pdfUrl && !string.IsNullOrWhiteSpace(pdfUrl))
                                {
                                    var res = await TryDownloadUrlAsPdfWithNameAsync(client, pdfUrl, doi, crossrefTitle, cancellationToken).ConfigureAwait(false);
                                    if (res != null) return res;
                                }

                                if (TryGetString(best, "url") is string url && !string.IsNullOrWhiteSpace(url))
                                {
                                    var res = await TryDownloadUrlAsPdfWithNameAsync(client, url, doi, crossrefTitle, cancellationToken).ConfigureAwait(false);
                                    if (res != null) return res;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignore unpaywall errors
                    }
                }

                return null;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (disposeClient) client?.Dispose();
            }

            static bool IsPdfResponse(HttpContentHeaders headers)
            {
                if (headers.ContentType?.MediaType != null && headers.ContentType.MediaType.Contains("pdf", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (headers.ContentDisposition?.FileName != null && headers.ContentDisposition.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (headers.ContentDisposition?.FileNameStar != null && headers.ContentDisposition.FileNameStar.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }

            static async Task<PdfFetchResult?> TryDownloadUrlAsPdfWithNameAsync(HttpClient client, string url, string doi, string? crossrefTitle, CancellationToken ct)
            {
                try
                {
                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
                    using var resp = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
                    if (!resp.IsSuccessStatusCode) return null;
                    if (IsPdfResponse(resp.Content.Headers))
                    {
                        var bytes = await resp.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
                        var name = GetFileNameFromResponse(resp, doi, crossrefTitle) ?? MakeSafeFileNameFromDoi(doi);
                        return new PdfFetchResult(bytes, name);
                    }

                    var cd = resp.Content.Headers.ContentDisposition;
                    if (cd?.FileName != null && cd.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        var bytes = await resp.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
                        var name = TrimQuotes(cd.FileName);
                        name = SanitizeFileName(name);
                        return new PdfFetchResult(bytes, name);
                    }

                    // fallback: if URL ends with .pdf
                    var pathName = resp.RequestMessage?.RequestUri?.LocalPath;
                    if (!string.IsNullOrEmpty(pathName) && pathName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        var bytes = await resp.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
                        var name = Path.GetFileName(pathName);
                        name = SanitizeFileName(name);
                        return new PdfFetchResult(bytes, name);
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            }

            static async Task<PdfFetchResult?> TryDownloadIfPdfUrlWithNameAsync(HttpClient client, Uri uri, string doi, string? crossrefTitle, CancellationToken ct)
            {
                return await TryDownloadUrlAsPdfWithNameAsync(client, uri.ToString(), doi, crossrefTitle, ct).ConfigureAwait(false);
            }

            static string? GetFileNameFromResponse(HttpResponseMessage resp, string doi, string? crossrefTitle)
            {
                // Try Content-Disposition
                var cd = resp.Content.Headers.ContentDisposition;
                if (cd != null)
                {
                    var name = cd.FileNameStar ?? cd.FileName;
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        name = TrimQuotes(name);
                        return SanitizeFileName(name);
                    }
                }

                // Try the response URI path
                var uri = resp.RequestMessage?.RequestUri;
                if (uri != null)
                {
                    var pathName = uri.LocalPath;
                    if (!string.IsNullOrEmpty(pathName))
                    {
                        var candidate = Path.GetFileName(pathName);
                        if (!string.IsNullOrWhiteSpace(candidate) && candidate.IndexOf('.') >= 0)
                        {
                            return SanitizeFileName(candidate);
                        }
                    }
                }

                // Use Crossref title if available
                if (!string.IsNullOrWhiteSpace(crossrefTitle))
                {
                    var t = SanitizeFileName(crossrefTitle);
                    if (!t.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        t += ".pdf";
                    return t;
                }

                // fallback to DOI-based name
                return MakeSafeFileNameFromDoi(doi);
            }

            static string MakeSafeFileNameFromDoi(string doi)
            {
                var candidate = doi.Replace("/", "_").Replace("\\", "_");
                if (!candidate.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    candidate += ".pdf";
                return SanitizeFileName(candidate);
            }

            static string TrimQuotes(string s) => s?.Trim().Trim('"').Trim('\'') ?? s;

            static string SanitizeFileName(string name)
            {
                if (string.IsNullOrWhiteSpace(name)) return "file.pdf";
                var invalid = Path.GetInvalidFileNameChars();
                var cleaned = new string(name.Where(c => !invalid.Contains(c)).ToArray());
                // collapse spaces
                cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
                // shorten to reasonable length
                if (cleaned.Length > 200) cleaned = cleaned[..200].Trim();
                // ensure extension
                if (!cleaned.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    cleaned += ".pdf";
                return cleaned;
            }

            static string? TryGetString(JsonElement el, string prop)
            {
                if (el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String)
                    return v.GetString();
                return null;
            }
        }
    }
}