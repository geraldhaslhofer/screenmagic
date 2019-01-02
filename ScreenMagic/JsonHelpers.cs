
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace ScreenMagic
{


    class JsonHelpers
    {
        private static int ScaleInt(int origValue, double scaleFactor)
        {
            int value = (int)((double)origValue * scaleFactor);
            if (value == 0 && origValue != 0) return origValue;

            return value;
        }
        public static void GetBoundingBoxes(JToken token, List<BoundingBox> boxes, double scaleFactor)
        {
            foreach (var child in token.Children())
            {
                string t2 = child.Type.ToString();
                if (t2 == "Property")
                {
                    JProperty p = (JProperty)child;
                    if (p.Name == "boundingBox")
                    {
                        string rem = p.Value.ToString();
                        var items = rem.Split(',');
                        BoundingBox aBox = new BoundingBox();
                        aBox.X = ScaleInt(int.Parse(items[0]), scaleFactor);
                        aBox.Y = ScaleInt(int.Parse(items[1]), scaleFactor);
                        aBox.Width = ScaleInt(int.Parse(items[2]), scaleFactor);
                        aBox.Height = ScaleInt(int.Parse(items[3]), scaleFactor);
                        boxes.Add(aBox);
                    }

                    //Console.WriteLine(p.Value);

                }

                GetBoundingBoxes(child, boxes, scaleFactor);
            }
        }

        public static void Explode(JToken token, StringBuilder b)
        {
            foreach (var child in token.Children())
            {

                string t2 = child.Type.ToString();

                if (t2 == "Property")
                {
                    JProperty p = (JProperty)child;
                    //System.Diagnostics.Debug.WriteLine("==>hit:  " + p.Name);
                    if (p.Name == "text")
                    {
                        //Console.WriteLine(p.Value);
                        b.AppendLine(p.Value.ToString());
                    }

                };
                //System.Diagnostics.Debug.WriteLine(child.Type.ToString() + ":" + child + "::" + child.ToString());
                //System.Diagnostics.Debug.WriteLine("------------------------------");
                Explode(child, b);
            }
        }

        public static void GetTextElements(JToken token, OcrResults results, double scaleFactor)
        {
            foreach (var child in token.Children())
            {

                string t2 = child.Type.ToString();

                if (t2 == "Property")
                {
                    JProperty p = (JProperty)child;
                    //System.Diagnostics.Debug.WriteLine("==>hit:  " + p.Name);
                    if (p.Name == "words")
                    {
                        GetDetails(p, results, scaleFactor);
                        //Console.WriteLine(p.Value);

                    }

                };
                //System.Diagnostics.Debug.WriteLine(child.Type.ToString() + ":" + child + "::" + child.ToString());
                //System.Diagnostics.Debug.WriteLine("------------------------------");
                GetTextElements(child, results, scaleFactor);
            }
        }

        private static void SetBoundingBox(OcrResult res, string box, double scaleFactor)
        {
            string rem = box.Replace('{', ' ');
            rem = rem.Replace('}', ' ');
            var items = rem.Split(',');
            res.X = ScaleInt(int.Parse(items[0]), scaleFactor);
            res.Y = ScaleInt(int.Parse(items[1]), scaleFactor);
            res.Width = ScaleInt(int.Parse(items[2]), scaleFactor);
            res.Height = ScaleInt(int.Parse(items[3]), scaleFactor);
        }
        private static void GetDetails(JToken token, OcrResults results, double scaleFactor)
        {
            foreach (var child in token.Children())
            {
                foreach (var twoChild in child.Children())
                {
                    string t2 = twoChild.Type.ToString();
                    OcrResult ocrResult = new OcrResult();

                    foreach (var aDetail in twoChild.Children())
                    {
                        if (((JProperty)aDetail).Name == "boundingBox")
                        {
                            SetBoundingBox(ocrResult, ((JProperty)aDetail).Value.ToString(), scaleFactor);
                        }
                        if (((JProperty)aDetail).Name == "text")
                        {
                            ocrResult.Text = ((JProperty)aDetail).Value.ToString();
                        }
                    }
                    results.Results.Add(ocrResult);
                }
            }

        }


    }
}