using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace BibLib.Daos
{
    public class PaperColumnDao : HomlDao<PaperColumnDto>
    {
        public static readonly PaperColumnDao Instance = new();

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
            try
            {
                PaperColumnDto newValue = value;
                newValue.Paper = paper.Id;
                return Instance.Insert(newValue);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private static IList Parse(Paper paper, IEnumerable<PaperColumn> values)
        {
            var response = new List<PaperColumnDto>();

            foreach (var value in values) {
                response.Add(Parse(value, paper.Id));
            }

            return response;
        }

        private static PaperColumnDto Parse(PaperColumn value, int? id)
        {
            try
            {
                PaperColumnDto response = value;
                response.Paper = id;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static long Create(Paper paper, IEnumerable<PaperColumn> values)
        {
            try
            {
                if (values.Any())
                {
                    var newValues = Parse(paper, values);
                    return Instance.Insert(newValues);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
