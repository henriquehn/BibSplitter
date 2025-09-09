namespace BibLib.Parsing
{
    public class BibToken
    {
        public BibTokenType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public int Length => Value?.Length ?? 0;

        public int Position { get; set; }
    }
}
