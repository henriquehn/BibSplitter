using System.Text;
using System.Text.RegularExpressions;

namespace BibLib.Utils
{
    public static class PdfUtils
    {
        // Returns the page count or null if it cannot be determined.
        public static int? GetPageCount(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;
            var bytes = File.ReadAllBytes(filePath);
            return GetPageCount(bytes);
        }

        // Returns the page count from PDF bytes without third-party libraries.
        public static int? GetPageCount(byte[] pdfBytes)
        {
            try
            {
                if (pdfBytes == null || pdfBytes.Length == 0) return null;

                // Use Latin1 to preserve raw byte values in a reversible manner.
                var text = Encoding.Latin1.GetString(pdfBytes);

                // 1) Try to find /Root reference in the trailer (most reliable).
                var rootMatch = Regex.Match(text, @"/Root\s+(\d+)\s+(\d+)\s+R", RegexOptions.RightToLeft);
                if (rootMatch.Success)
                {
                    if (TryResolvePagesCountFromReferencedObject(text, rootMatch.Groups[1].Value, rootMatch.Groups[2].Value, out int pages))
                    {
                        return pages;
                    }
                }

                // 2) Fallback: Try to find any /Pages object reference and resolve it.
                var pagesRefMatch = Regex.Match(text, @"/Pages\s+(\d+)\s+(\d+)\s+R", RegexOptions.Singleline);
                if (pagesRefMatch.Success)
                {
                    if (TryResolvePagesCountFromReferencedObject(text, pagesRefMatch.Groups[1].Value, pagesRefMatch.Groups[2].Value, out int pages))
                    {
                        return pages;
                    }
                }

                // 3) Final fallback: search the whole file for "/Count <number>" and return the largest value found.
                var countMatches = Regex.Matches(text, @"/Count\s+(\d+)");
                if (countMatches.Count > 0)
                {
                    var max = countMatches.Select(m => int.Parse(m.Groups[1].Value)).Max();
                    return max;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // Helper: given an object number and generation, find that object's text and extract /Pages ref or /Count.
        private static bool TryResolvePagesCountFromReferencedObject(string pdfText, string objNum, string genNum, out int pages)
        {
            pages = 0;

            try
            {
                // Find the object definition: "<objNum> <genNum> obj" ... "endobj"
                var patternObj = $@"\b{Regex.Escape(objNum)}\s+{Regex.Escape(genNum)}\s+obj\b(.*?)\bendobj\b";
                var objMatch = Regex.Match(pdfText, patternObj, RegexOptions.Singleline | RegexOptions.Compiled);
                if (!objMatch.Success) return false;

                var objContent = objMatch.Groups[1].Value;

                // If this object directly contains /Count, return it.
                var countMatch = Regex.Match(objContent, @"/Count\s+(\d+)");
                if (countMatch.Success)
                {
                    pages = int.Parse(countMatch.Groups[1].Value);
                    return true;
                }

                // If this object references a Pages object, follow it.
                var pagesRefMatch = Regex.Match(objContent, @"/Pages\s+(\d+)\s+(\d+)\s+R");
                if (pagesRefMatch.Success)
                {
                    var pagesObjNum = pagesRefMatch.Groups[1].Value;
                    var pagesGenNum = pagesRefMatch.Groups[2].Value;
                    // Resolve the referenced Pages object
                    var patternPagesObj = $@"\b{Regex.Escape(pagesObjNum)}\s+{Regex.Escape(pagesGenNum)}\s+obj\b(.*?)\bendobj\b";
                    var pagesObjMatch = Regex.Match(pdfText, patternPagesObj, RegexOptions.Singleline | RegexOptions.Compiled);
                    if (pagesObjMatch.Success)
                    {
                        var pagesObjContent = pagesObjMatch.Groups[1].Value;
                        var countMatch2 = Regex.Match(pagesObjContent, @"/Count\s+(\d+)");
                        if (countMatch2.Success)
                        {
                            pages = int.Parse(countMatch2.Groups[1].Value);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}