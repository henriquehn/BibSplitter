using BibLib.Parsing;

namespace BibLib.Extensions
{
    public  static class BibPaeringExtensions
    {
        public static string Join(this IEnumerable<BibTokenType> types, string separator)
        {
            return string.Join(separator, types.Select(t => t.ToString()));
        }

        public static string AsTableRow<T>(this T entity)
        {
            var props = typeof(T).GetProperties();
            var values = new List<string>();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(Attributes.TableColumnAttribute), false).FirstOrDefault() is Attributes.TableColumnAttribute columnAttr)
                {
                    var value = prop.GetValue(entity)?.ToString() ?? string.Empty;
                    values.Add(columnAttr.FormatValue(value));
                }
            }
            return string.Join(" | ", values);
        }
        public static string AsCsvRow<T>(this T entity)
        {
            var props = typeof(T).GetProperties();
            var values = new List<string>();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(Attributes.TableColumnAttribute), false).FirstOrDefault() is Attributes.TableColumnAttribute columnAttr)
                {
                    var value = prop.GetValue(entity)?.ToString() ?? string.Empty;
                    values.Add(columnAttr.FormatCsvValue(value));
                }
            }
            return string.Join(";", values);
        }

        public static string AsTableHeader<T>()
        {
            var props = typeof(T).GetProperties();
            var values = new List<string>();
            var lines = new List<string>();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(Attributes.TableColumnAttribute), false).FirstOrDefault() is Attributes.TableColumnAttribute columnAttr)
                {
                    values.Add(columnAttr.FormatLabel());
                    lines.Add(columnAttr.FormatLine());
                }
            }

            var lineString = string.Join("---", lines);
            var headerString = string.Join(" | ", values);
            return $"{lineString}\r\n{headerString}\r\n{lineString}";
        }

        public static string AsCsvTableHeader<T>()
        {
            var props = typeof(T).GetProperties();
            var values = new List<string>();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(Attributes.TableColumnAttribute), false).FirstOrDefault() is Attributes.TableColumnAttribute columnAttr)
                {
                    values.Add(columnAttr.FormatCsvLabel());
                }
            }

            var headerString = string.Join(";", values);
            return headerString;
        }

        public static string AsTabFooter<T>(string content)
        {
            var props = typeof(T).GetProperties();
            var lines = new List<string>();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(Attributes.TableColumnAttribute), false).FirstOrDefault() is Attributes.TableColumnAttribute columnAttr)
                {
                    lines.Add(columnAttr.FormatLine());
                }
            }

            var lineString = string.Join("---", lines);
            return $"{lineString}\r\n{content}\r\n{lineString}";
        }
    }
}
