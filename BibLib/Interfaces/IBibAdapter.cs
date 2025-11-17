using System.Data;
using System.Text;

namespace BibLib.Interfaces
{
    public interface IBibAdapter<T, L> where L : new()
    {
        public IBibRecord Parse(T value);
        public L CreateList();
        public IEnumerable<T> GetEnumerator(L entries);
        public IEnumerable<IBibRecord> GetRecords(L entries);
        void AppendEntry(IBibRecord entry, L entries);
        public void AppendEntries<T1, L1, A> (L1 src, DataTable dst, A adapter) where L1 : new() where A: IBibAdapter<T1, L1>;
        void Serialize(T entry, StringBuilder sb);
        int Count(L entries);
    }
}
