using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels.PaperManager
{
    [SourceTable("sources")]
    public class Source
    {
        [SourceField("id")]
        [FieldBehaviour(FieldIndexType.PrimaryIndex, FieldEditMode.AutoIncrementField)]
        public int? Id { get; set; }
        [SourceField("name")]
        public string Name { get; set; }
        [SourceField("alias")]
        public string Alias { get; set; }
    }
}
