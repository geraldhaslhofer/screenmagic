
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
        public static void GetBoundingBoxes(JToken token, List<BoundingBox> boxes)
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
                        aBox.X = int.Parse(items[0]);
                        aBox.Y = int.Parse(items[1]);
                        aBox.Width = int.Parse(items[2]);
                        aBox.Height = int.Parse(items[3]);
                        boxes.Add(aBox);
                    }

                    //Console.WriteLine(p.Value);

                }

                GetBoundingBoxes(child, boxes);
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

        public static void GetTextElements(JToken token, OcrResults results)
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
                        GetDetails(p, results);
                        //Console.WriteLine(p.Value);

                    }

                };
                //System.Diagnostics.Debug.WriteLine(child.Type.ToString() + ":" + child + "::" + child.ToString());
                //System.Diagnostics.Debug.WriteLine("------------------------------");
                GetTextElements(child, results);
            }
        }

        private static void SetBoundingBox(OcrResult res, string box)
        {
            string rem = box.Replace('{', ' ');
            rem = rem.Replace('}', ' ');
            var items = rem.Split(',');
            res.X = int.Parse(items[0]);
            res.Y = int.Parse(items[1]);
            res.Width = int.Parse(items[2]);
            res.Height = int.Parse(items[3]);
        }
        private static void GetDetails(JToken token, OcrResults results)
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
                            SetBoundingBox(ocrResult, ((JProperty)aDetail).Value.ToString());
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