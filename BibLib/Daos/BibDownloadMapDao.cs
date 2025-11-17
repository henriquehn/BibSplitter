using BibLib.DataModels;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class BibDownloadMapDao : HomlDao<BibDownloadMap>
    {
        public BibDownloadMapDao() : base(ConfigurationHelper.Get("MainDataSourceName")) { }

        private static BibDownloadMapDao instance = new BibDownloadMapDao();

        public static long Create(params BibDownloadMap[] items)
        {
            return instance.Insert(items);
        }

        public static IList<BibDownloadMap> GetAll()
        {
            return instance.Select<List<BibDownloadMap>>();
        }

        public static IList<BibDownloadMap> Get(BibDownloadMap filter)
        {
            return instance.Select<List<BibDownloadMap>>(filter);
        }

        public static BibDownloadMap GetOne(BibDownloadMap filter)
        {
            return instance.SelectOne(filter);
        }

    }
}
