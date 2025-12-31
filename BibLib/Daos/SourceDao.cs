using BibLib.Collections;
using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class SourceDao : HomlDao<Source>
    {
        public static SourceDao Instance { get; } = new();
        public static SourceHolder Sources { get; } = SourceDao.CreateHolder();

        public SourceDao() : base(ConfigurationHelper.Get("PaperManager"))
        {
        }

        public static SourceHolder CreateHolder()
        {
            var response = new SourceHolder();
            foreach (Source source in Instance.Select())
            {
                response.Add(source);
            }
            return response;
        }

        public class SourceHolder: ElementHolder<Source>
        {
            protected override string GetName(Source element)
            {
                return element.Name;
            }

            protected override int GetId(Source element)
            {
                return element.Id ?? -1;
            }

            protected override Source CreateNew(string name)
            {
                var newValue = new Source { Name = name };
                Instance.Insert(newValue);
                return newValue;
            }
        }
    }
}
