using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class PaperColumnDao : HomlDao<PaperColumnDto>
    {
        public static PaperColumnDao Instance { get; } = new PaperColumnDao();

        public PaperColumnDao() : base(ConfigurationHelper.Get("PaperManager"))
        {
        }

        public static IEnumerable<PaperColumn> ReadByPaper(int? id)
        {
            foreach (var item in Instance.Select<List<PaperColumnDto>>(new PaperColumnDto { Paper = id }))
            {
                yield return item;
            }
        }

        public static long Create(Paper paper, PaperColumn value)
        {
            PaperColumnDto newValue = value;
            newValue.Paper = paper.Id;
            return Instance.Insert(newValue);
        }

        private static IEnumerable<PaperColumnDto> Parse(Paper paper, IEnumerable<PaperColumn> values)
        {
            foreach (var value in values) {
                PaperColumnDto newValue = value;
                newValue.Paper = paper.Id;
                yield return newValue;
            }
        }
        public static long Create(Paper paper, IEnumerable<PaperColumn> values)
        {
            return Instance.Insert(Parse(paper, values));
        }
    }
}
