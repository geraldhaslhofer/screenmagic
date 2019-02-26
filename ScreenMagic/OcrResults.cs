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

        //public bool IsFullyContainedInRectangle(int x1, int y1, int x2, int y2)
        //{
        //    return ((x1 <= X) && (y1 <= Y) && (x2 >= X) && (y2 >= Y));
        //}
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

        public string GetRawText()
        {
            StringBuilder b = new StringBuilder();
            foreach (var item in Results)
            {
                b.Append(item.Text);
                b.Append(" ");
            }
            return b.ToString();
        }

        public BoundingBox GetSmallestBoundingBox(int x, int y)
        {


            int smallestArea = int.MaxValue;
            BoundingBox res = null;

            foreach (var result in BoundingBoxes)
            {
                if (result.ContainsCoordinate(x, y))
                {
                    int area = result.Height * result.Width;
                    //smallest?
                    if (area < smallestArea)
                    {
                        smallestArea = area;
                        res = result;
                    }
                }
            }
            return res;
        }

        public List<OcrResult> GetOcrResultFromBoundingbox(BoundingBox b)
        {
            List<OcrResult> l = new List<OcrResult>();

            foreach (var result in Results)
            {
                if (result.IsInBoundingBox(b)) l.Add(result);
            }
            return l;
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

        public bool IsInBoundingBox(BoundingBox b)
        {
            return ((X >= b.X) && (X + Width <= b.X + b.Width)) && ((Y >= b.Y) && (Y + Height <= b.Y + b.Height));
            
        }
    }

}