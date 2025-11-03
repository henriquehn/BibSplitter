using Microsoft.Win32;

namespace BibSettings
{
    public static class BibSettingsHolder
    {
        private const string RegistryBasePath = @"Software\{0}\";

        public static string Load(DefaultBibSettings settings, string defaultValue = null, string appName = "BibSettingsHolder")
        {
            var registryPath=string.Format(RegistryBasePath, appName);
            using (var key = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                return key?.GetValue(settings.ToString()) as string ?? defaultValue;
            }
        }
        public static void Save(DefaultBibSettings settings, string value, string appName = "BibSettingsHolder")
        {
            var registryPath=string.Format(RegistryBasePath, appName);
            using (var key = Registry.CurrentUser.CreateSubKey(registryPath))
            {
                key.SetValue(settings.ToString(), value, RegistryValueKind.String);
            }
        }
    }
}
