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
    }

}