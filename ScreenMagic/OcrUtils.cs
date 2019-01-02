using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class OcrUtils
    {
        public async static Task<OcrResults> GetOcrResults(byte[] jpegEncoded, double scaleFactor)
        {
            IOcrResultProvider anOCR = OcrProviderFactory.GetOcrResultsProvider();

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
