using BibLib.DataModels.BibDownload;
using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class PaperDao: HomlDao<Paper>
    {
        public static readonly PaperDao Instance = new();
        public static readonly Dictionary<string, Paper> PaperStore = BuildStore();

        public PaperDao() : base(ConfigurationHelper.Get("PaperManager"))
        {
        }

        private static Dictionary<string, Paper> BuildStore()
        {
            var response = new Dictionary<string, Paper>(StringComparer.OrdinalIgnoreCase);
            var papers = Instance.Select<List<Paper>>();
            foreach (var paper in papers)
            {
                response[paper.Hash] = paper;
            }
            return response;
        }

        public static long Create(Paper value)
        {
            if (PaperStore.TryGetValue(value.Hash, out var paper))
            {
                value.Id = paper.Id;
                return 0;
            }
            else
            { 
                var response = Instance.Insert(value);
                PaperColumnDao.Create(value, value.Columns);
                return response;
            }
        }

        public static IEnumerable<Paper> Create(BibElements values, IList<Duplicate> duplicates)
        {
            try
            {
                var papers = values.AsPapers();
                Create(papers, duplicates);
                return papers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static long Create(IEnumerable<Paper> values, IList<Duplicate> duplicates = null)
        {
            long response = 0;
            foreach (var paper in values)
            {
                if (PaperStore.TryGetValue(paper.Hash, out var existingPaper))
                {
                    paper.Id = PaperStore[paper.Hash].Id;
                    Duplicate duplicate = paper;
                    duplicates?.Add(paper);
                }
                else
                {
                    response += Instance.Insert(paper);
                    PaperColumnDao.Create(paper, paper.Columns);
                    PaperStore.Add(paper.Hash, paper);
                }
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
