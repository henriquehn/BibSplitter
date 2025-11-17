using System.Collections.Specialized;
using System.Configuration;

namespace BibLib.Utils
{
    public static class ConfigurationHelper
    {
        public static string Get(string section)
        {
            return Get(section, "");
        }

        public static string Get(string section, string defaultValue)
        {
            string result = ConfigurationManager.AppSettings.Get(section);
            if (string.IsNullOrEmpty(result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static IDictionary<string, string> GetSection(string section)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var elements = ConfigurationManager.GetSection(section) as NameValueCollection;
                foreach (string key in elements)
                {
                    if (!result.ContainsKey(key))
                    {
                        var value = elements[key];
                        result.Add(key, value);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return result;
        }
    }
}
