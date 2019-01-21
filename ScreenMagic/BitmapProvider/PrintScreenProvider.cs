using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ScreenMagic
{
    class PrintScreenProvider : IBitmapProvider
    {
        Bitmap IBitmapProvider.CaptureScreenshot()
        {
            var screen = Utils.CaptureScreenshot(Config.WindowToWatch);
            return screen;
        }
    }
}
