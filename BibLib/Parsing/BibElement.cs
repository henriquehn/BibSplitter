namespace BibLib.Parsing
{
    public class BibElement : SortedList<string, string>
    {
        public BibType Type { get; set; }
        public string Key { get; set; }

        public BibElement() : base(StringComparer.OrdinalIgnoreCase) { }
    }
}
