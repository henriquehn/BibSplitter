using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels.PaperManager
{
    [SourceTable("columns")]
    public class ColumnDto
    {
        [SourceField("id")]
        [FieldBehaviour(FieldIndexType.PrimaryIndex,  FieldEditMode.AutoIncrementField)]
        public int? Id { get; set; }
        [SourceField("name")]
        public string Name { get; set; }
    }
}
