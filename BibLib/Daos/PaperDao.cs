using BibLib.DataModels.BibDownload;
using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class PaperDao: HomlDao<Paper>
    {
        public static PaperDao Instance { get; } = new PaperDao();

        public PaperDao() : base(ConfigurationHelper.Get("PaperManager"))
        {
        }

        public static long Create(Paper value)
        {
            var response = Instance.Insert(value);
            PaperColumnDao.Create(value, value.Columns);
            return response;

        }

        public static IEnumerable<Paper> Create(BibElements values)
        {
            var papers = values.AsPapers();
            Create(papers);
            return papers;
        }


        public static long Create(IEnumerable<Paper> values)
        {
            var response = Instance.Insert(values);
            foreach (var paper in values)
            {
                PaperColumnDao.Create(paper, paper.Columns);
            }
            return response;
        }

        public static List<Paper> ReadAll()
        {
            var response = Instance.Select<List<Paper>>();
            foreach (var paper in response)
            {
                paper.Columns.AddRange(PaperColumnDao.ReadByPaper(paper.Id));
            }
            return response;
        }

        public static Paper ReadOne(int id)
        {
            var response = Instance.SelectOne(new Paper { Id = id });
            response.Columns.AddRange(PaperColumnDao.ReadByPaper(response.Id));
            return response;
        }
    }
}
