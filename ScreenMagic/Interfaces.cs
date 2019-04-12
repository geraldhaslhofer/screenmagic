using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace ScreenMagic
{
    public class CaptureContext
    {
        public Screen CapturedOnScreen { get; set; }
        public double ScaleFactor { get; set; }
        public Rectangle CapturedScreenLogical { get; set; }
        public Rectangle CapturedWindowLogical { get; set; }
        public Rectangle CapturedWindowPhysicalDimension { get; set; }

    }

    interface IOcrResultProvider
    {
        Task<JToken> MakeOCRRequest(byte[] jpegEncoded);
    }
    interface IBitmapProvider
    {
        void CaptureScreenshot(out Bitmap bitmap, out CaptureContext captureContext);
        
    }
}
