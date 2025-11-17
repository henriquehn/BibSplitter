using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels
{
    [SourceTable("bib_download_map")]
    public class BibDownloadMap
    {
        [FieldBehaviour(FieldIndexType.PrimaryIndex, FieldEditMode.AutoIncrementField)]
        [SourceField("Id")]
        public int Id { get; set; }
        [SourceField("Doi")]
        public string Doi { get; set; }
        [SourceField("File_name")]
        public string FileName { get; set; }
        [SourceField("Page_count")]
        public int PageCount { get; set; }
        [SourceField("Created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
