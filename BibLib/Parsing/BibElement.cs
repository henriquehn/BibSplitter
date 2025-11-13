using System.Text.Json.Serialization;

namespace BibLib.Parsing
{
    public class BibElement : SortedList<string, string>
    {
        public BibType Type { get; set; }
        [JsonIgnore]
        public int? PageCount { get; set; }
        [JsonIgnore]
        public string Title { get; set; }
        [JsonIgnore]
        public string Authors { get; set; }
        public string Key { get; set; }

        public BibElement() : base(StringComparer.OrdinalIgnoreCase) { }
    }
}
