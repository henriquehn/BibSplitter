using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace BibLib.Parsing
{
    public static class BibConverter
    {
        public static IEnumerable<BibElement> Deserialize(string data)
        {
            var parser = new BibParser(data);
            return parser.Parse();
        }

        public static string Serialize(IEnumerable<BibElement> entries)
        {
            var sb = new StringBuilder();
            foreach (var entry in entries)
            {
                sb.AppendLine($"@{entry.Type.ToString().ToLower()}{{{entry.Key},");
                foreach (var field in entry)
                {
                    sb.AppendLine($"  {field.Key.ToLower()} = {{{field.Value}}},");
                }
                sb.AppendLine("}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static IEnumerable<BibElement> FromCsvFile(string filePath, bool hasHeader = true, string delimiter = ",")
        {
            var content = File.ReadAllText(filePath);
            return FromCsvData(content, hasHeader, delimiter);
        }

        public static IEnumerable<BibElement> FromCsvData(string fileContent, bool hasHeader = true, string delimiter = ",")
        {
            var response = new List<BibElement>();
            var hedarManager = new HeaderManager();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
            {
                using (var parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(delimiter);
                    bool isFirstRow = hasHeader;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (isFirstRow)
                        {
                            hedarManager.AddHeaders(fields);
                            isFirstRow = false;
                        }
                        else
                        {
                            var entry = new BibElement()
                            {
                                Type = BibType.Article,
                                Key = Guid.NewGuid().ToString().Replace("-", "")
                            };

                            for (int index = 0; index < fields.Length; index++)
                            {
                                if (!string.IsNullOrWhiteSpace(fields[index]))
                                {
                                    var header = hedarManager.GetHeader(index);
                                    if (string.Equals(header, "type", StringComparison.OrdinalIgnoreCase))
                                    {
                                        entry.Type = Enum.TryParse<BibType>(fields[index], true, out var bibType) ? bibType : BibType.Article;
                                    }
                                    else
                                    {
                                        entry[header] = fields[index];
                                    }
                                }
                            }
                            response.Add(entry);
                        }
                    }
                }
            }
            return response;
        }
    }
}
