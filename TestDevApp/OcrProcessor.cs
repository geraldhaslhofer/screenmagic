using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace TestDevApp
{
    class OcrProcessor
    {
        public static async Task OcrPicture(string source, string destination)
        {


            
            
            byte[] content = Fileutils.GetFileFromPath(source);
            MemoryStream m = new MemoryStream(content);
            var bitmap = Fileutils.DeserializeJpeg(m);

            OcrResults ocrResults = await OcrSupport.GetOcrResults(content, 1.0);
            SemanticRegions r = new SemanticRegions();
            r.Regions = new List<SemanticRegion>();

            //Add bounding rectangle info
            SemanticRegion circum = new SemanticRegion();
            circum.SemanticType = SemanticType.Border;
            circum.Box = new BoundingBox();
            circum.Box.X = 0;
            circum.Box.Y = 0;
            circum.Box.Width = bitmap.Width;
            circum.Box.Height= bitmap.Height;

            r.Regions.Add(circum);

            foreach (var res in ocrResults.BoundingBoxes)
            {
                SemanticRegion aRegion = new SemanticRegion();
                aRegion.Box = res;
                aRegion.SemanticType = SemanticType.Text;
                r.Regions.Add(aRegion);
            }

            Fileutils.SerializeSemanticRegions(r, destination);
        }
    }
}

