using BibLib.Parsing;
using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels.PaperManager
{
    [SourceTable("papers")]
    public class PaperBase
    {
        [SourceField("id")]
        [FieldBehaviour(FieldIndexType.PrimaryIndex)]
        public int? Id { get; set; }
        [SourceField("bib_type")]
        public BibType Type { get; set; }
        [SourceField("bib_key")]
        public string Key { get; set; }
        [SourceField("title")]
        public string Title { get; set; }
        [SourceField("page_count")]
        public int? PageCount { get; set; }
        [SourceField("ahthor")]
        public string Author { get; set; }
        [SourceField("year")]
        public int Year { get; set; }
        [SourceField("doi")]
        public string Doi { get; set; }
        [SourceField("keywords")]
        public string Keywords { get; set; }
        [SourceField("publisher")]
        public string Publisher { get; set; }
        [SourceField("source_id")]
        public int? Source { get; set; }
    }
}