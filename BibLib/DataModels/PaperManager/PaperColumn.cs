using BibLib.Daos;

namespace BibLib.DataModels.PaperManager
{
    public class PaperColumn
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static implicit operator PaperColumnDto(PaperColumn column)
        {
            return new PaperColumnDto
            {
                Column = ColumnDao.Columns[column.Name],
                Value = column.Value
            };
        }
    }
}