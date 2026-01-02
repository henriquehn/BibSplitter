using BibLib.DataModels.PaperManager;
using BibLib.Utils;
using DodFramework.DataLibrary.DAO.Homl;

namespace BibLib.Daos
{
    public class DuplicateDao : HomlDao<Duplicate>
    {
        public static readonly DuplicateDao Instance = new();
        public static readonly Dictionary<string, Duplicate> DuplicatesStore = BuildStore();

        public DuplicateDao() : base(ConfigurationHelper.Get("PaperManager"))
        {
        }

        private static Dictionary<string, Duplicate> BuildStore()
        {
            var response = new Dictionary<string, Duplicate>(StringComparer.OrdinalIgnoreCase);
            var duplicates = Instance.Select<List<Duplicate>>();
            foreach (var duplicate in duplicates)
            {
                response[duplicate.Hash] = duplicate;
            }
            return response;
        }

        public static long Create(Duplicate value)
        {
            if (DuplicatesStore.TryGetValue(value.Hash, out var duplicate))
            {
                value.Id = duplicate.Id;
                return 0;
            }
            else
            {
                var response = Instance.Insert(value);
                DuplicatesStore.Add(value.Hash, value);
                return response;
            }
        }

        public static long Create(IEnumerable<Duplicate> values)
        {
            long response = 0;
            foreach (var duplicate in values)
            {
                /*
                 * Quando a duplicata é criada, ela recebe uma sequência única.
                 * Essa sequência, combinada com o hash do paper, forma um hash exclusivo para a duplicata.
                 * Isso torna improvável a duplicação do hash da duplicata, mas não impossível.
                 * Essa rotina garante que não haverá duplicação.
                 */
                while (DuplicatesStore.ContainsKey(duplicate.Hash))
                {
                    duplicate.GenerateSequence();
                }
                response += Instance.Insert(duplicate);
                DuplicatesStore.Add(duplicate.Hash, duplicate);
            }
            return response;
        }

        public static List<Duplicate> ReadAll()
        {
            return Instance.Select<List<Duplicate>>();
        }

        public static Duplicate ReadOne(int id)
        {
            return Instance.SelectOne(new Duplicate { Id = id });
        }
    }
}
