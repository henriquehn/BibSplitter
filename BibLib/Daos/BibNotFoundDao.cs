using BibLib.DataModels.BibDownload;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class BibNotFoundDao: HomlDao<BibNotFound>
    {
        public BibNotFoundDao() : base(ConfigurationHelper.Get("MainDataSourceName")) { }

        private static BibNotFoundDao instance = new();

        public static long Create(params BibNotFound[] items)
        {
            return instance.Insert(items);
        }

        public static long Replace(params BibNotFound[] items)
        {
            return instance.Update(items);
        }

        public static long SetDeleted(string doi)
        {
            var existing = instance.SelectOne(new BibNotFound() { Doi = doi });
            if (existing == null)
            {
                existing.Deleted = true;
                return instance.Update(existing);
            }
            return 0;
        }

        public static long Remove(BibNotFound item)
        {
            return instance.Delete(item);
        }

        public static long CountAll()
        {
            return instance.Count();
        }

        public static long CountNotDeleted()
        {
            return instance.Count(new BibNotFound() { Deleted = false});
        }

        public static IList<BibNotFound> GetAll()
        {
            return instance.Select<List<BibNotFound>>();
        }

        public static IList<BibNotFound> GetNotDeleted()
        {
            return instance.Select<List<BibNotFound>>(new BibNotFound() { Deleted = false });
        }

        public static IDictionary<string, BibNotFound> GetDictionary()
        {
            var response = new Dictionary<string, BibNotFound>(StringComparer.OrdinalIgnoreCase);
            var results = GetAll();
            foreach (var item in results)
            {
                if (!string.IsNullOrEmpty(item.Doi?.Trim()))
                {
                    response[item.Doi] = item;
                }
            }
            return response;
        }

        public static IDictionary<string, BibNotFound> GetNotDeletedDictionary()
        {
            var response = new Dictionary<string, BibNotFound>(StringComparer.OrdinalIgnoreCase);
            var results = GetNotDeleted();
            foreach (var item in results)
            {
                if (!string.IsNullOrEmpty(item.Doi?.Trim()))
                {
                    response[item.Doi] = item;
                }
            }
            return response;
        }

        public static IList<BibNotFound> Get(BibNotFound filter)
        {
            return instance.Select<List<BibNotFound>>(filter);
        }

        public static BibNotFound GetOne(BibNotFound filter)
        {
            return instance.SelectOne(filter);
        }

    }
}
