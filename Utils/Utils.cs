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
            return MonitorInfo.ScaleRect(r, scale);
        }

        public static double GetScaleFactorForScreen(Screen screen)
        {
            return MonitorInfo.GetScaleFactorForScreen(screen);
        }

        public static Bitmap CaptureScreenFromRectPhysical(Screen screen, Rectangle rPhysical)
        {
            return ScreenGrab.CaptureScreenFromRectPhysical(screen, rPhysical);
        }

        //Return the full bitmap 
        public static Bitmap GetBitmapForScreen(Screen screen)
        {
            double scaleFactor = MonitorInfo.GetScaleFactorForScreen(screen);
            return ScreenGrab.CaptureScreen(screen, scaleFactor);
        }

        public static void LocateProcessWindowRelativePhysical(IntPtr handle, out Screen screen, out Rectangle windowRelativePhysical)
        {
            //Determine position in logical coordinate space
            Rectangle rectLogicalWindow = MonitorInfo.GetWindowRectLogical(handle);
            //figure out which screen it is on
            //Screen has logical coordinates
            screen = MonitorInfo.GetScreenForRectLogical(rectLogicalWindow);

            Rectangle windowRelativeLogical = MonitorInfo.GetRelativeRectangle(screen.Bounds, rectLogicalWindow);
            windowRelativePhysical = MonitorInfo.ScaleRect(windowRelativeLogical, MonitorInfo.GetScaleFactorForScreen(screen));
        }


        //Process related Utils
        public static IEnumerable<DesktopWindow> GetActiveWindows()
        {
            var windows = MonitorInfo.GetDesktopWindows();
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
