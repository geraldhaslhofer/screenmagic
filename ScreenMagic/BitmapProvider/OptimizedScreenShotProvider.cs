using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenMagic
{
    class OptimizedScreenShotProvider : IBitmapProvider
    {
        private Screen _lastCapturedScreen;

        Bitmap IBitmapProvider.CaptureScreenshot()
        {
            Rectangle rPhysical;
            GlobalUtils.Utils.LocateProcessWindowRelativePhysical(Config.WindowToWatch, out _lastCapturedScreen, out rPhysical);
            return GlobalUtils.Utils.CaptureScreenFromRectPhysical(_lastCapturedScreen, rPhysical);
        }
        double IBitmapProvider.ScaleFactor()
        {
            return GlobalUtils.Utils.GetScaleFactorForScreen(_lastCapturedScreen);
        }
    }
}
