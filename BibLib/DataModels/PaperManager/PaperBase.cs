using BibLib.Parsing;
using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels.PaperManager
{
    [SourceTable("papers")]
    public class PaperBase
    {
        [SourceField("id")]
        [FieldBehaviour(FieldIndexType.PrimaryIndex, FieldEditMode.AutoIncrementField)]
        public int? Id { get; set; }
        [SourceField("bib_type")]
        public BibType? BibElementType { get; set; }
        [SourceField("bib_key")]
        public string BibElemenKey { get; set; }
        [SourceField("title")]
        public string Title { get; set; }
        [SourceField("page_count")]
        public int? PageCount { get; set; }
        [SourceField("author")]
        public string Author { get; set; }
        [SourceField("year")]
        public int? Year { get; set; }
        [SourceField("doi")]
        public string Doi { get; set; }
        [SourceField("keywords")]
        public string Keywords { get; set; }
        [SourceField("publisher")]
        public string Publisher { get; set; }
        [SourceField("source_id")]
        public int? Source { get; set; }
        [SourceField("bib_file_id")]
        public int? File { get; set; }
        [SourceField("hash")]
        [FieldBehaviour(FieldIndexType.ExclusiveIndex)]
        public string Hash { get; set; }
    }
}