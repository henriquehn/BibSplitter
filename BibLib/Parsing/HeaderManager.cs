namespace BibLib.Parsing
{
    internal class HeaderManager
    {
        private List<string> headers = new();
        private readonly Dictionary<string, string> headerMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Authors", "author" },
            { "Publication Year", "year" },
            { "Journal/Book", "journal" },
            { "create date", "date" },
            { "first author", "firstauthor" },
            { "content type", "type" },
            { "item doi", "doi" },
            { "item title", "title" },
            { "publication title", "journal" },
        };

        public HeaderManager()
        {
        }

        public void AddHeader(string header)
        {
            var alias = headerMap.ContainsKey(header) ? headerMap[header] : header;
            if (!headers.Contains(alias, StringComparer.OrdinalIgnoreCase))
            {
                headers.Add(alias);
            }
        }

        public void AddHeaders(IEnumerable<string> headers)
        {
            foreach (var header in headers)
            {
                AddHeader(header);
            }
        }

        public string GetHeader(int index)
        {
            if (index<0)
            {
                throw new ArgumentException("Index cannot be negative.");
            }
            if (index >= headers.Count)
            {
                return $"Column{index}";
            }
            return headers[index];
        }
    }
}