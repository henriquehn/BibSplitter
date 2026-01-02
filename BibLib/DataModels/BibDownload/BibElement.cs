using BibLib.Daos;
using BibLib.DataModels.PaperManager;
using BibLib.Interfaces;
using BibLib.Parsing;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text.Json.Serialization;

namespace BibLib.DataModels.BibDownload
{
    public class BibElement : SortedList<string, string>, IBibRecord
    {
        private string identifier = null;

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

        public string Source { get; set; }
        public string File { get; set; }

        public string Hash { get; set; }

        public BibElement() : base(StringComparer.OrdinalIgnoreCase) { }

        public static implicit operator Paper(BibElement element)
        {
            var response = new Paper() {
                BibElementType = element.Type,
                BibElemenKey = element.Key,
                Source = SourceDao.Sources[element.Source],
                File = BibFileDao.BibFiles[element.File],
                Hash = element.Hash,
                PageCount = element.PageCount ?? 0
            };

            foreach (var item in element)
            {
                response.AppendColumn(item.Key, item.Value);
            }

            /* Garante que campos importantes não ficarão nulos */
            response.Year ??= 0;
            response.Keywords ??= "";
            response.Author ??= "";
            response.Abstract ??= "";
            response.Publisher ??= "";
            response.Title ??= "";


            return response;
        }

        public void CreateHash()
        {
            var hashData = System.Text.Encoding.UTF8.GetBytes($"{this.Title}|{this.Authors}|{this.Doi}|{this.Source}");
            this.Hash = DodFramework.Security.Algorithms.Encription.ComputeSHA256String(hashData);
        }

        public void CountPages()
        {
            if (this.PageCount == null)
            {
                if (this.TryGetValue("pagecount", out string pageCountString))
                {
                    if (int.TryParse(pageCountString, out int pageCount))
                    {
                        this.PageCount = pageCount;
                        return;
                    }
                    else
                    { 
                        this.PageCount = -1;
                    }
                }
                else if (this.TryGetValue("pages", out string pages))
                {
                    var parts = pages.Split(new char[] { '-', '–' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 && int.TryParse(parts[0], out int startPage) && int.TryParse(parts[1], out int endPage))
                    {
                        this.PageCount = endPage - startPage + 1;
                    }
                    else
                    {
                        this.PageCount = -1;
                    }
                }
            }
        }
    }
}
