using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
    
}
