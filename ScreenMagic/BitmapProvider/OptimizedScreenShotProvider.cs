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
        

        void IBitmapProvider.CaptureScreenshot(out Bitmap bitmap, out CaptureContext captureContext)
        {
            CaptureContext context = new CaptureContext();

            Screen lastCapturedScreen;
            Rectangle windowAbsoluteLogical;
            Rectangle windowRelativeLogical;
            Rectangle windowRelativePhysical;
            double scaleFactor;

            //public static void LocateProcessWindowRelativePhysical(IntPtr handle, out Screen screen, out Rectangle windowAbsoluteLogical, out Rectangle windowRelativeLogical, out Rectangle windowRelativePhysical, out double scaleFactor)

            GlobalUtils.Utils.LocateProcessWindowRelativePhysical(Config.WindowToWatch, out lastCapturedScreen, out windowAbsoluteLogical, out windowRelativeLogical, out windowRelativePhysical, out scaleFactor);
            
            bitmap = GlobalUtils.Utils.CaptureScreenFromRectPhysical(lastCapturedScreen, windowRelativePhysical);
            context.CapturedOnScreen = lastCapturedScreen;
            context.CapturedScreenLogical = lastCapturedScreen.Bounds;
            context.CapturedWindowLogical = windowAbsoluteLogical;
            context.CapturedWindowPhysicalDimension = windowRelativePhysical;
            context.ScaleFactor = scaleFactor;

            captureContext = context;
            
        }
        
    }
}
