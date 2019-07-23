using ScreenMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Utils
{
    public class OcrSupport
    {

        public async static Task<OcrResults> GetOcrResults(byte[] jpegEncoded, double scaleFactor)
        {
            CognitiveVisionOcrProvider anOCR = new CognitiveVisionOcrProvider();

            var x = await anOCR.MakeOCRRequest(jpegEncoded);
            OcrResults results = new OcrResults();
            results.Results = new List<OcrResult>();
            List<BoundingBox> boxes = new List<BoundingBox>();
            JsonHelpers.GetBoundingBoxes(x, boxes, scaleFactor);
            JsonHelpers.GetTextElements(x, results, scaleFactor);
            results.BoundingBoxes = boxes;
            return results;
        }

    }
}

