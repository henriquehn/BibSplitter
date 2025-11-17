using BibLib.Adapters;
using BibLib.DataModels;
using BibLib.Extensions;
using BibLib.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace BibLib.Parsing
{
    public static class BibConverter
    {
        public static string Serialize(IEnumerable<IBibRecord> entries, Action<int> progressCallback = null)
        {
            var sb = new StringBuilder();

            progressCallback ??= (_)=>{};

            int totalCount = entries.Count();
            int currentCount = 0;

            progressCallback.InvokeAsync(0);
            foreach (var entry in entries)
            {
                currentCount++;
                progressCallback.InvokeAsync((int)((currentCount / (double)totalCount) * 100));
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

        public static void Serialize(this IEnumerable<IBibRecord> entries, string filename, Action<int> progressCallback = null)
        {
            File.WriteAllText(filename, Serialize(entries, progressCallback));
        }

        public static IEnumerable<IBibRecord> Deserialize(string data, Action<int> progressCallback = null)
        {
            return BibConverter<BibElement, BibElements, BibElementAdapter>.Deserialize(data, progressCallback);
        }
        public static IEnumerable<IBibRecord> DeserializeFile(string data, Action<int> progressCallback = null)
        {
            return Deserialize(File.ReadAllText(data), progressCallback);
        }

    }

    public static class BibConverter<T, L, A> where A : IBibAdapter<T, L>, new() where L : new()
    {
        private static readonly A adapter = new();

        public static L Deserialize(string data, Action<int> progressCallback = null)
        {
            var parser = new BibParser<T, L, A>(data, adapter, progressCallback);
            return parser.Parse();
        }

        public static string Serialize(L entries, Action<int> progressCallback = null)
        {
            var sb = new StringBuilder();

            progressCallback ??= (_)=>{};

            int totalCount = adapter.Count(entries);
            int currentCount = 0;

            progressCallback.InvokeAsync(0);
            foreach (var entry in adapter.GetEnumerator(entries))
            {
                currentCount++;
                progressCallback.InvokeAsync((int)((currentCount / (double)totalCount) * 100));
                adapter.Serialize(entry, sb);
            }
            return sb.ToString();
        }

        public static L FromCsvFile(string filePath, bool hasHeader = true, string delimiter = ",", Action<int> progressCallback = null)
        {
            var content = File.ReadAllText(filePath);
            return FromCsvData(content, hasHeader, delimiter, progressCallback);
        }

        public static L FromCsvData(string fileContent, bool hasHeader = true, string delimiter = ",", Action<int> progressCallback = null)
        {
            progressCallback ??= (_)=>{};
            var response = adapter.CreateList();
            var headerManager = new HeaderManager();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
            {
                long totalCount = stream.Length;
                int currentCount = 0;
                progressCallback.InvokeAsync(0);
                using (var parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(delimiter);
                    bool isFirstRow = hasHeader;
                    while (!parser.EndOfData)
                    {
                        currentCount = (int)stream.Position;
                        progressCallback.InvokeAsync((int)((currentCount / (double)totalCount) * 100));
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
            progressCallback.InvokeAsync(100);
            return response;
        }
    }
}
