using BibLib.DataModels.PaperManager;
using BibLib.Interfaces;
using BibLib.Parsing;
using System.Text.Json.Serialization;

namespace BibLib.DataModels.BibDownload
{
    public class BibElement : SortedList<string, string>, IBibRecord
    {
        public BibType Type { get; set; }
        [JsonIgnore]
        public int? PageCount { get; set; }
        [JsonIgnore]
        public string Title
        {
            get
            {
                TryGetValue("title", out string title);
                return title;
            }
            set
            {
                this["title"] = value;
            }
        }

        [JsonIgnore]
        public string Authors
        {
            get
            {
                TryGetValue("author", out string authors);
                return authors;
            }
            set
            {
                this["author"] = value;
            }
        }
        public string Key { get; set; }
        public string Doi
        {
            get
            {
                TryGetValue("doi", out string doi);
                return doi;
            }
            set
            {
                this["doi"] = value;
            }
        }

        public string Abstract { 
            get
            {
                TryGetValue("abstract", out string abs);
                return abs;
            }
            set
            {
                this["abstract"] = value;
            }
        }

        public BibElement() : base(StringComparer.OrdinalIgnoreCase) { }

        public static implicit operator Paper(BibElement element)
        {
            var response = new Paper();
            //{
            //    Type = element.Type,
            //    Key = element.Key,
            //    Title = element.Title,
            //    Abstract = element.Abstract,
            //    PageCount = element.PageCount ?? 0,
            //    Doi = element.Doi,
            //};

            foreach (var item in element)
            {
                response.AppendColumn(item.Key, item.Value);
            }

            return response;
        }
    }
}
