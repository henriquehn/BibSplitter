using BibLib.Attributes;
using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels
{
    [SourceTable("bib_not_found")]
    public class BibNotFound
    {
        [FieldBehaviour(FieldIndexType.PrimaryIndex, FieldEditMode.AutoIncrementField)]
        [SourceField("Id")]
        [TableColumn("ID",5)]
        public int? Id { get; set; }
        [SourceField("Doi")]
        [TableColumn("DOI", -28, csvStringQualifier: @"""")]
        public string Doi { get; set; }
        [SourceField("Created_at")]
        [TableColumn("Criado em", "dd/MM/yyyy")]
        public DateTime? CreatedAt { get; set; }

        public static implicit operator BibNotFound(BibElement other)
        {
            return new BibNotFound
            {
                Doi = other.DOI,
                CreatedAt = DateTime.Now
            };
        }
    }
}
