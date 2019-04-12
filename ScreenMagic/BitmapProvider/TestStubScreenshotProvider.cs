using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class TestStubScreenshotProvider : IBitmapProvider
    {
        Bitmap IBitmapProvider.CaptureScreenshot()
        {
            string testimagePath = Path.Combine(Utils.GetAssemblyPath(), "Assets\\test.jpg");
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(testimagePath);
            return bitmap;

            
        }
        double IBitmapProvider.ScaleFactor()
        {
            return 1;
        }

    }
}
