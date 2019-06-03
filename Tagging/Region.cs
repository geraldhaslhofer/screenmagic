using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Media;
namespace ScreenMagic
{

    public enum RegionKind
    {
        Background,
        Icon,
        Image,
        Text,
        UxElement
    }
    public class Region
    {
        private static char DELIMITER = ';';

        public Rectangle RegionRect { get; set;}

        public RegionKind Kind {get;set;}

        public string SerializeToCsv()
        {
            StringBuilder b = new StringBuilder();
            b.Append(Kind.ToString()); b.Append(DELIMITER);
            b.Append(RegionRect.X.ToString()); b.Append(DELIMITER);
            b.Append(RegionRect.Y.ToString()); b.Append(DELIMITER);
            b.Append(RegionRect.Width.ToString()); b.Append(DELIMITER);
            b.Append(RegionRect.Height.ToString());
            return b.ToString();
        }

        public static System.Windows.Media.Brush GetBrushFromRegionKind(RegionKind kind)
        {
            switch (kind)
            {
                case RegionKind.Background: return System.Windows.Media.Brushes.Transparent;
                case RegionKind.Icon: return System.Windows.Media.Brushes.Yellow;
                case RegionKind.Image: return System.Windows.Media.Brushes.OrangeRed;
                case RegionKind.Text: return System.Windows.Media.Brushes.LightBlue;
                case RegionKind.UxElement: return System.Windows.Media.Brushes.Black;
                default: throw new NotSupportedException("Unknown kind");
            }
        }

    }

}
