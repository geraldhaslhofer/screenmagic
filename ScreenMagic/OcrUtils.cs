using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class OcrUtils
    {
        public async static Task<OcrResults> GetOcrResults(byte[] jpegEncoded)
        {
            IOcrResultProvider anOCR = OcrProviderFactory.GetOcrResultsProvider();

            var x = await anOCR.MakeOCRRequest(jpegEncoded);
            OcrResults results = new OcrResults();
            results.Results = new List<OcrResult>();
            List<BoundingBox> boxes = new List<BoundingBox>();
            JsonHelpers.GetBoundingBoxes(x, boxes);
            JsonHelpers.GetTextElements(x, results);
            results.BoundingBoxes = boxes;
            return results;
        }

    }
}
