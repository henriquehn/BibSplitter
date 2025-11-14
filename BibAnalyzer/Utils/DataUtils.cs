namespace BibAnalyzer.Utils
{
    public static class DataUtils
    {
        public static string BuildBibFilter(string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
            {
                return string.Empty;
            }
            var sanitizedCriteria = SanitizeFilterString(criteria);
            return $"Title LIKE '%{sanitizedCriteria}%' OR Author LIKE '%{sanitizedCriteria}%'";
        }

        private static string SanitizeFilterString(string input)
        {
            return input.Replace("'", "''")
                        .Replace("[", "[[]")
                        .Replace("%", "[%]")
                        .Replace("*", "[*]")
                        .Replace("_", "[_]");
        }
    }
}
