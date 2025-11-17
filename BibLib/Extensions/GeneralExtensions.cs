using BibLib.Attributes;
using System.ComponentModel;
using System.Drawing;

namespace BibLib.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetDescription(this Enum value)
        {
            try
            {
                var fi = value.GetType().GetField(value.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
            }
            catch
            {
                return value.ToString();
            }
        }

        public static Color GetStatusColor(this Enum value, Color? defaultColor = null)
        {
            try
            {
                var fi = value.GetType().GetField(value.ToString());
                var attributes = (StatusColorAttribute[])fi.GetCustomAttributes(typeof(StatusColorAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Color : defaultColor ?? SystemColors.ControlText;
            }
            catch
            {
                return defaultColor ?? SystemColors.ControlText;
            }
        }

        public static void InvokeAsync<T>(this Action<T> action, T args)
        {
            Task.Run(() => {
                try
                {
                    action(args);
                }
                catch { }
            });
        }
    }
}
