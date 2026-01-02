using BibLib.Daos;
using BibLib.DataModels.BibDownload;
using BibLib.Parsing;
using DodFramework.BaseTypes.Helper;
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

        public void AppendColumn(string name, string value)
        {
            try
            {
                if (properties.TryGetValue(name, out var prop))
                {
                    prop.SetValue(this, TypeHelper.DirectValueCast(prop.PropertyType, value, TypeHelper.GetnNullValue(prop.PropertyType)));
                }
                else
                {
                    Columns.Add(new PaperColumn() { Name = name, Value = value });
                }
            }
            catch { }
        }

        public static implicit operator BibElement(Paper paper)
        {
            var response = new BibElement
            {
                Type = paper.BibElementType ?? BibType.Misc,
                Key = paper.BibElemenKey,
                Title = paper.Title,
                Abstract = paper.Abstract,
                PageCount = paper.PageCount,
                Doi = paper.Doi,
                Source = SourceDao.Sources[paper.Source ?? -1],
                File = BibFileDao.BibFiles[paper.File ?? -1],
            };
            response["author"] = paper.Author;
            response["year"] = paper.Year.ToString();
            response["keywords"] = paper.Keywords;
            response["publisher"] = paper.Publisher;
            foreach (var column in paper.Columns)
            {
                response[column.Name] = column.Value;
            }
            response.Hash = paper.Hash;
            return response;
        }

        public static implicit operator Duplicate(Paper paper)
        {
            var response = new Duplicate()
            {
                BibElementType = paper.BibElementType,
                BibElemenKey = paper.BibElemenKey,
                Title = paper.Title,
                Abstract = paper.Abstract,
                PageCount = paper.PageCount,
                Author = paper.Author,
                Year = paper.Year,
                Doi = paper.Doi,
                Keywords = paper.Keywords,
                Publisher = paper.Publisher,
                Source = paper.Source,
                File = paper.File,
                Hash = paper.Hash,
                PaperHash = paper.Hash,
            };
            response.GenerateSequence();
            return response;
        }

    }
}
