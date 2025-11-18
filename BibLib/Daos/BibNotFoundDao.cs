using BibLib.DataModels;
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

        public static long Remove(BibNotFound item)
        {
            return instance.Delete(item);
        }

        public static long CountAll()
        {
            return instance.Count();
        }

        public static IList<BibNotFound> GetAll()
        {
            return instance.Select<List<BibNotFound>>();
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
