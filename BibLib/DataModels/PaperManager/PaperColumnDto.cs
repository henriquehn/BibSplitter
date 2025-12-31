using BibLib.Daos;
using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels.PaperManager
{
    [SourceTable("paper_columns")]
    public class PaperColumnDto 
    {
        [SourceField("paper_id")]
        public int? Paper { get; set; }
        [SourceField("column_id")]
        public int? Column { get; set; }
        [SourceField("value")]
        public string Value { get; set; }

        public static implicit operator PaperColumn(PaperColumnDto dto)
        {
            return new PaperColumn
            {
                Name = ColumnDao.Columns[dto.Column ?? -1],
                Value = dto.Value
            };
        }
    }
}
