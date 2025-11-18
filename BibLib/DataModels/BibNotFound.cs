using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels
{
    [SourceTable("bib_not_found")]
    public class BibNotFound
    {
        [FieldBehaviour(FieldIndexType.PrimaryIndex, FieldEditMode.AutoIncrementField)]
        [SourceField("Id")]
        public int? Id { get; set; }
        [SourceField("Doi")]
        public string Doi { get; set; }
        [SourceField("Created_at")]
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
