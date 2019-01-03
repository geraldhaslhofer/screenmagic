using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ScreenMagic
{
    [DataContract]
    public class BoundingBox
    { 
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }

        public bool ContainsCoordinate(int x, int y)
        {
            return ((x >= X && x <= (X + Width)) && ((y >= Y) && y <= (Y + Height)));
        }
    }

    [DataContract]
    public class OcrResults
    {
        [DataMember]
        public List<BoundingBox> BoundingBoxes { get; set; }

        [DataMember]
        public List<OcrResult> Results { get; set; }
        public bool ContainsText(string searchText)
        {
            foreach (var x in Results)
            {
                if (x.ContainsText(searchText)) return true;
            }
            return false;
        }
        public IEnumerable<OcrResult> GetMatchingTextResults(string searchText)
        {
            var x = from item in Results where item.ContainsText(searchText) select item;
            return x;
        }

        public OcrResult GetOcrResultFromCoordinate(int x, int y)
        {
            foreach (var result in Results)
            {
                if (result.ContainsCoordinate(x, y)) return result;
            }
            return null;
        }


    }
    [DataContract]
    public class OcrResult
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public string Text { get; set; }

        public bool ContainsText(string searchText)
        {
            return Text.ToLower().Contains(searchText.ToLower());
        }

        public bool ContainsCoordinate(int x, int y)
        {
            return ((x >= X && x <= (X + Width)) && ((y >= Y) && y <= (Y + Height)));
        }
    }

}