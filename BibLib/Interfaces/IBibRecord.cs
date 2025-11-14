using BibLib.Parsing;
using System.Text.Json.Serialization;

namespace BibLib.Interfaces
{
    public interface IBibRecord: ICollection<KeyValuePair<string, string>>
    {
        public BibType Type { get; set; }
        [JsonIgnore]
        public string Key { get; set; }
    }
}
