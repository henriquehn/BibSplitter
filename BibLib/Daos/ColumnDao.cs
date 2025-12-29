
using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class ColumnDao: HomlDao<ColumnDto>
    {
        public static ColumnDao Instance { get; } = new();
        public static ColumHolder Columns { get; } = ColumnDao.CreateHolder();


        public ColumnDao() : base(ConfigurationHelper.Get("MainDataSourceName"))
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

        public class ColumHolder
        {
            private Dictionary<int, string> ColumnsById = new();
            private Dictionary<string, int> ColumnsByNames = new();
            public void Add(ColumnDto column)
            {
                ColumnsById[column.Id ?? -1] = column.Name;
                ColumnsByNames[column.Name] = column.Id ?? -1;
            }

            public string this[int id]
            {
                get
                {
                    return ColumnsById[id];
                }
            }

            public int this[string name]
            {
                get
                {
                    ColumnsByNames.TryGetValue(name, out int id);
                    if (id < 1)
                    {
                        var newValue = new ColumnDto { Name = name };
                        Instance.Insert(newValue);
                        this.Add(newValue);
                        return newValue.Id ?? -1;
                    }
                    return id;
                }
            }
        }
    }
}
