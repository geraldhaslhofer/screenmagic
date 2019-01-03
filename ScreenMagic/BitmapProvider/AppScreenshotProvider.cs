using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class AppScreenshotProvider : IBitmapProvider
    {
       
        public AppScreenshotProvider()
        {
         }
        Bitmap IBitmapProvider.CaptureScreenshot()
        {
            var screen = Utils.CaptureScreenshot(Modes.WindowToWatch);
            return screen;
        }
    }
}
