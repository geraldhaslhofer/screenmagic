using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GlobalUtils
{
    public class Utils
    {
        public static Rectangle ScaleRect(Rectangle r, double scale)
        {
            return MonitorHelper.ScaleRect(r, scale);
        }

        public static double GetScaleFactorForScreen(Screen screen)
        {
            return MonitorHelper.GetScaleFactorForScreen(screen);
        }

        public static Bitmap CaptureScreenFromRectPhysical(Screen screen, Rectangle rPhysical)
        {
            return ScreenGrab.CaptureScreenFromRectPhysical(screen, rPhysical);
        }

        //Return the full bitmap 
        public static Bitmap GetBitmapForScreen(Screen screen)
        {
            double scaleFactor = MonitorHelper.GetScaleFactorForScreen(screen);
            return ScreenGrab.CaptureScreen(screen, scaleFactor);
        }

        public static void LocateProcessWindowRelativePhysical(IntPtr handle, out Screen screen, out Rectangle windowAbsoluteLogical, out Rectangle windowRelativeLogical, out Rectangle windowRelativePhysical, out double scaleFactor)
        {
            //Determine position in logical coordinate space
            windowAbsoluteLogical = MonitorHelper.GetWindowRectLogical(handle);
            //figure out which screen it is on
            //Screen has logical coordinates
            screen = MonitorHelper.GetScreenForRectLogical(windowAbsoluteLogical);

            windowRelativeLogical = MonitorHelper.GetRelativeRectangle(screen.Bounds, windowAbsoluteLogical);
            scaleFactor = MonitorHelper.GetScaleFactorForScreen(screen);
            windowRelativePhysical = MonitorHelper.ScaleRect(windowRelativeLogical, scaleFactor);
        }


        //Process related Utils
        public static IEnumerable<DesktopWindow> GetActiveWindows()
        {
            var windows = MonitorHelper.GetDesktopWindows();
            var visible = from x in windows where x.IsVisible && x.Title.Length > 2 orderby x.Title select x;
            return visible;
        }

        public static DesktopWindow GetYourPhoneWindow()
        {
            var windows = GetActiveWindows();
            var yourPhone = from x in windows where x.Title.ToString().ToLower().Contains("your phone") select x;
            if (yourPhone == null || yourPhone.Count() == 0) return null;
            return yourPhone.First();
        }
    }
}
