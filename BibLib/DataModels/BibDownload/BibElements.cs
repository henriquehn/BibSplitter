using BibLib.DataModels.PaperManager;
using BibLib.Interfaces;

namespace BibLib.DataModels.BibDownload
{
    public class BibElements : List<BibElement>, IEnumerable<IBibRecord>
    {
        IEnumerator<IBibRecord> IEnumerable<IBibRecord>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<Paper> AsPapers()
        {
            foreach (var element in this)
            {
                yield return Parse(element);
            }
        }

        private static Paper Parse(BibElement value)
        {
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
