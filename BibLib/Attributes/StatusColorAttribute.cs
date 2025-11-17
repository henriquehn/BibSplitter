using System.Drawing;

namespace BibLib.Attributes
{
    public class StatusColorAttribute : Attribute
    {
        public Color Color { get; }

        public StatusColorAttribute(int r, int g, int b)
        {
            this.Color = Color.FromArgb(r, g, b);
        }
        public StatusColorAttribute(int argb)
        {
            this.Color = Color.FromArgb(argb);
        }
        public StatusColorAttribute(string colorName)
        {
            this.Color = Color.FromName(colorName);
        }
    }
}