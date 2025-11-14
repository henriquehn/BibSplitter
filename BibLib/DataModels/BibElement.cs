using BibLib.Interfaces;
using BibLib.Parsing;
using System.Text.Json.Serialization;

namespace BibLib.DataModels
{
    public class BibElement : SortedList<string, string>, IBibRecord
    {
        public BibType Type { get; set; }
        [JsonIgnore]
        public int? PageCount { get; set; }
        [JsonIgnore]
        public string Title { get; set; }
        [JsonIgnore]
        public string Authors { get; set; }
        public string Key { get; set; }
        public string DOI { get; set; }

        public BibElement() : base(StringComparer.OrdinalIgnoreCase) { }
    }
}
