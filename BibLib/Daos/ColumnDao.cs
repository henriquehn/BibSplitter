
using BibLib.Collections;
using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class ColumnDao: HomlDao<ColumnDto>
    {
        public static ColumnDao Instance { get; } = new();
        public static ColumHolder Columns { get; } = ColumnDao.CreateHolder();


        public ColumnDao() : base(ConfigurationHelper.Get("PaperManager"))
        {
        }

        public static ColumHolder CreateHolder()
        {
            var response = new ColumHolder();
            foreach (ColumnDto column in Instance.Select())
            {
                response.Add(column);
            }
            return response;
        }

        public class ColumHolder : ElementHolder<ColumnDto>
        {
            protected override string GetName(ColumnDto element)
            {
                return element.Name;
            }

            protected override int GetId(ColumnDto element)
            {
                return element.Id ?? -1;
            }

            protected override ColumnDto CreateNew(string name)
            {
                var newValue = new ColumnDto { Name = name };
                Instance.Insert(newValue);
                return newValue;
            }
        }
    }
}
