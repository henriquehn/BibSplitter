using BibLib.DataModels.BibDownload;
using BibLib.Interfaces;
using BibLib.Parsing;
using System.Data;
using System.Text;

namespace BibLib.Adapters
{
    public class BibTableAdapter : IBibAdapter<DataRow, DataTable>
    {

        public IBibRecord Parse(DataRow value)
        {
            var response = new BibElement();
            foreach (DataColumn field in value.Table.Columns)
            {
                if (string.Equals(field.ColumnName, "Type", StringComparison.OrdinalIgnoreCase))
                {
                    response.Type = Enum.TryParse<BibType>(value[field].ToString(), true, out var bibType) ? bibType : BibType.Misc;
                }
                else if (string.Equals(field.ColumnName, "Key", StringComparison.OrdinalIgnoreCase))
                {
                    response.Key = value[field].ToString();
                }
                else if (value[field] != DBNull.Value)
                {
                    response.Add(field.ColumnName, value[field].ToString());
                }
            }
            response.CreateHash();
            response.CountPages();
            return response;
        }
        public void AppendEntry(IBibRecord entry, DataTable entries)
        {
            foreach (var item in entry)
            {
                if (!entries.Columns.Contains(item.Key))
                {
                    entries.Columns.Add(item.Key, typeof(string));
                }
            }
            var row = entries.Rows.Add();
            row["key"] = entry.Key;
            row["type"] = entry.Type.ToString();
            foreach (var item in entry)
            {
                row[item.Key] = item.Value;
            }
            //entries.Rows.Add(row);
        }

        public void AppendEntries<T1, L1, A>(L1 src, DataTable dst, A adapter)
            where L1 : new()
            where A : IBibAdapter<T1, L1>
        {
            foreach (var entry in adapter.GetEnumerator(src))
            {
                var record = adapter.Parse(entry);
                AppendEntry(record, dst);
            }
        }

        public DataTable CreateList()
        {
            var response = new DataTable();
            response.Columns.Add("Type", typeof(string));
            response.Columns.Add("Key", typeof(string));
            return response;
        }

        public IEnumerable<DataRow> GetEnumerator(DataTable entries)
        {
            foreach (DataRow row in entries.Rows)
            {
                yield return row;
            }
        }
        public IEnumerable<IBibRecord> GetRecords(DataTable entries)
        {
            foreach (DataRow row in entries.Rows)
            {
                var record = Parse(row);
                yield return record;
            }
        }

        public void Serialize(DataRow entry, StringBuilder sb)
        {
            sb.AppendLine($"@{entry["Type"].ToString().ToLower()}{{{entry["Key"]},");
            foreach (DataColumn field in entry.Table.Columns)
            {
                if ((!(string.Equals(field.ColumnName,"Type", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(field.ColumnName, "Key", StringComparison.OrdinalIgnoreCase))) &&
                    (entry[field] != DBNull.Value))
                {
                    sb.AppendLine($"  {field.ColumnName.ToLower()} = {{{entry[field]}}},");
                }
            }
            sb.AppendLine("}");
            sb.AppendLine();

        }

        public int Count(DataTable entries)
        {
            return entries.Rows.Count;
        }
    }
}
