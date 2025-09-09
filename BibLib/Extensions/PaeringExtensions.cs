using BibLib.Parsing;

namespace BibLib.Extensions
{
    public  static class PaeringExtensions
    {
        public static string Join(this IEnumerable<BibTokenType> types, string separator)
        {
            return string.Join(separator, types.Select(t => t.ToString()));
        }
    }
}
