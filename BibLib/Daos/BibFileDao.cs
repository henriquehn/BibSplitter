using BibLib.Collections;
using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class BibFileDao : HomlDao<BibFile>
    {
        public static readonly BibFileDao Instance = new();
        public static readonly BibFileDaoHolder BibFiles = CreateHolder();

        public BibFileDao() : base(ConfigurationHelper.Get("PaperManager"))
        {
        }
        public static BibFileDaoHolder CreateHolder()
        {
            var response = new BibFileDaoHolder();
            foreach (BibFile file in Instance.Select())
            {
                response.Add(file);
            }
            return response;
        }

        public class BibFileDaoHolder : ElementHolder<BibFile>
        {
            protected override string GetName(BibFile element)
            {
                return element.Name;
            }

            protected override int GetId(BibFile element)
            {
                return element.Id ?? -1;
            }

            protected override BibFile CreateNew(string name)
            {
                var newValue = new BibFile { Name = name };
                Instance.Insert(newValue);
                return newValue;
            }
        }

        public static bool Exists(string name)
        {
            return Instance.SelectOne(new BibFile { Name = name }) != null;
        }
    }
}
