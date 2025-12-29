using BibLib.Interfaces;

namespace BibLib.DataModels.BibDownload
{
    public class BibElements : List<BibElement>, IEnumerable<IBibRecord>
    {
        IEnumerator<IBibRecord> IEnumerable<IBibRecord>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
