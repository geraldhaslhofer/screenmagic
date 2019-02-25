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
            var screen = LowLevelUtils.GetBitmapFromHwnd(Config.WindowToWatch);
            return screen;
        }
    }
}
