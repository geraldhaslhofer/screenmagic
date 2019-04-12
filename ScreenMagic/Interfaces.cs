using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ScreenMagic
{
    interface IOcrResultProvider
    {
        Task<JToken> MakeOCRRequest(byte[] jpegEncoded);
    }
    interface IBitmapProvider
    {
        Bitmap CaptureScreenshot();
        double ScaleFactor();
    }
}
