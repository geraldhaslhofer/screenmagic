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
        private IntPtr _windowToWatch = IntPtr.Zero;

        public AppScreenshotProvider()
        {
            _windowToWatch = Utils.GetWindowHandleFromApp("outlook");
        }
        Bitmap IBitmapProvider.CaptureScreenshot()
        {
            var screen = Utils.CaptureScreenshot(_windowToWatch);
            return screen;
        }
    }
}
