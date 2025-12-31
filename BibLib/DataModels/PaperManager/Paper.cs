using BibLib.DataModels.BibDownload;
using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;
using System.Reflection;

namespace BibLib.DataModels.PaperManager
{

    [SourceTable("papers")]
    public class Paper : PaperBase
    {
        private static readonly Dictionary<string, PropertyInfo> properties = typeof(Paper).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(prop => prop.Name.ToLower(), prop => prop, StringComparer.OrdinalIgnoreCase);

        [SourceField("abstract")]
        public string Abstract { get; set; }
        [IgnoreField]
        public List<PaperColumn> Columns { get; } = new();

        public static implicit operator BibElement(Paper paper)
        {
            var respnse = new BibElement
            {
                Type = paper.Type,
                Key = paper.Key,
                Title = paper.Title,
                Abstract = paper.Abstract,
                PageCount = paper.PageCount,
                Doi = paper.Doi,
            };
            respnse["author"] = paper.Author;
            respnse["year"] = paper.Year.ToString();
            respnse["keywords"] = paper.Keywords;
            respnse["publisher"] = paper.Publisher;
            foreach (var column in paper.Columns)
            {
                respnse[column.Name] = column.Value;
            }
            return respnse;
        }

        public void AppendColumn(string name, string value)
        {
            try
            {
                if (properties.TryGetValue(name, out var prop))
                {
                    prop.SetValue(this, Convert.ChangeType(value, prop.PropertyType));
                }
                else
                {
                    Columns.Add(new PaperColumn() { Name = name, Value = value });
                }
            }
            catch { }
        }

    }
}
