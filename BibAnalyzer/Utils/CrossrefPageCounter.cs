using System.Text.Json;
using System.Text.RegularExpressions;

namespace BibAnalyzer.Utils
{
    public static class CrossrefPageCounter
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
        public static async Task<int?> GetPageCountFromDoiAsync(string doi, HttpClient client = null)
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
    }
}