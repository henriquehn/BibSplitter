using BibLib.Daos;

namespace BibLib.DataModels.PaperManager
{
    public class PaperColumnDto 
    {
        public int? Paper { get; set; }
        public int? Column { get; set; }
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
