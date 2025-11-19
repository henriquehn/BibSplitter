using System.ComponentModel.DataAnnotations;

namespace BibLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute: Attribute
    {
        private readonly string displayName;
        private readonly int columnSize;
        private readonly string format;
        private readonly string csvStringQualifier;

        public TableColumnAttribute(string displayName, int columnSize, string format = null, string csvStringQualifier = "")
        {
            this.displayName = displayName;
            this.columnSize = columnSize;
            this.format = format;
            this.csvStringQualifier = csvStringQualifier;
        }
        public TableColumnAttribute(string displayName, string format):this(displayName, -format.Length, format)
        {
        }

        public String FormatValue(object value)
        {
            var response = $"{value}";
            response = this.format == null ? string.Format($"{{0,{columnSize}}}", value) : string.Format($"{{0,{columnSize}:{this.format}}}", value);
            response = response.Substring(0, Math.Abs(columnSize));
            return response;
        }

        public String FormatCsvValue(object value)
        {
            var response = $"{value}";
            if (!string.IsNullOrEmpty(csvStringQualifier))
            {
                response = response.Replace(csvStringQualifier, $"{csvStringQualifier}{csvStringQualifier}");
            }
            response = this.format == null ? $"{value}" : string.Format($"{{0:{this.format}}}", value);
            return $"{csvStringQualifier}{response}{csvStringQualifier}";
        }

        public String FormatLabel()
        {
            var response = $"{this.displayName}";
            response = this.format == null ? string.Format($"{{0,{columnSize}}}", response) : string.Format($"{{0,{columnSize}:{this.format}}}", response);
            response = response.Substring(0,Math.Abs(columnSize));
            return response;
        }

        public String FormatCsvLabel()
        {
            var response = @$"""{this.displayName}""";
            return response;
        }

        public String FormatLine()
        {
            return new string('-', Math.Abs(columnSize));
        }
    }
}
