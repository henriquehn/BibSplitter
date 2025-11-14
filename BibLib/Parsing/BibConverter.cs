using BibLib.DataModels;
using BibLib.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace BibLib.Parsing
{
    public static class BibConverter<T, L, A> where A : IBibAdapter<T, L>, new() where L : new()
    {
        private static readonly A adapter = new();

        public static L Deserialize(string data)
        {
            var parser = new BibParser<T, L, A>(data, adapter);
            return parser.Parse();
        }

        public static string Serialize(L entries)
        {
            var sb = new StringBuilder();
            foreach (var entry in adapter.GetEnumerator(entries))
            {
                adapter.Serialize(entry, sb);
            }
            return sb.ToString();
        }

        public static L FromCsvFile(string filePath, bool hasHeader = true, string delimiter = ",")
        {
            var content = File.ReadAllText(filePath);
            return FromCsvData(content, hasHeader, delimiter);
        }

        public static L FromCsvData(string fileContent, bool hasHeader = true, string delimiter = ",")
        {
            var response = adapter.CreateList();
            var headerManager = new HeaderManager();
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
                            headerManager.AddHeaders(fields);
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
                                    var header = headerManager.GetHeader(index);
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
                            adapter.AppendEntry(entry, response);
                        }
                    }
                }
            }
            return response;
        }
    }
}
