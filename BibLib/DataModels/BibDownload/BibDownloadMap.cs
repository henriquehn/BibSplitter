using BibLib.Attributes;
using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;
using System.Xml.Linq;

namespace BibLib.DataModels.BibDownload
{
    [SourceTable("bib_download_map")]
    public class BibDownloadMap
    {
        [FieldBehaviour(FieldIndexType.PrimaryIndex, FieldEditMode.AutoIncrementField)]
        [SourceField("Id")]
        [TableColumn("ID", 5)]
        public int? Id { get; set; }
        [SourceField("Doi")]
        [TableColumn("DOI", -28, csvStringQualifier: @"""")]
        public string Doi { get; set; }
        [SourceField("File_name")]
        [TableColumn("Arquivo", -130, csvStringQualifier: @"""")]
        public string FileName { get; set; }
        [SourceField("Page_count")]
        [TableColumn("Páginas", 7)]
        public int? PageCount { get; set; }
        [SourceField("Created_at")]
        [TableColumn("Criado em", "dd/MM/yyyy hh:nn:ss")]
        public DateTime? CreatedAt { get; set; }

        public static implicit operator BibDownloadMap(BibElement other)
        {
            return new BibDownloadMap
            {
                Doi = other.Doi,
                PageCount = other.PageCount ?? -1,
                CreatedAt = DateTime.Now
            };
        }

        public static implicit operator BibDownloadMap(BibNotFound other)
        {
            return new BibDownloadMap
            {
                Doi = other.Doi,
                CreatedAt = DateTime.Now
            };
        }

    }
}
