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
    }
}
