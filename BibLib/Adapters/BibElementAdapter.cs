using BibLib.DataModels;
using BibLib.Interfaces;
using System.Data;
using System.Text;

namespace BibLib.Adapters
{
    public class BibElementAdapter : IBibAdapter<BibElement, BibElements>
    {
        public IBibRecord Parse(BibElement value)
        {
            return value;
        }

        public void AppendEntry(IBibRecord entry, BibElements entries) => entries.Add((BibElement)entry);

        public void AppendEntries<T1, L1, A>(L1 src, DataTable dst, A adapter)
            where L1 : new()
            where A : IBibAdapter<T1, L1>
        {
            throw new NotImplementedException();
        }

        public BibElements CreateList()
        {
            return new BibElements();
        }

        public IEnumerable<BibElement> GetEnumerator(BibElements entries)
        {
            return entries;
        }

        public IEnumerable<IBibRecord> GetRecords(BibElements entries)
        {
            return entries;
        }

        public void Serialize(BibElement entry, StringBuilder sb)
        {
            sb.AppendLine($"@{entry.Type.ToString().ToLower()}{{{entry.Key},");
            foreach (var field in entry)
            {
                sb.AppendLine($"  {field.Key.ToLower()} = {{{field.Value}}},");
            }
            sb.AppendLine("}");
            sb.AppendLine();
        }
    }
}
