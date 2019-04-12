using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GlobalUtils
{
    class MonitorInfo
    {

        //Fiugre out the relative positining of inner within outer
        public static Rectangle GetRelativeRectangle(Rectangle outer, Rectangle inner)
        {
            Rectangle res = new Rectangle();
            res.X = inner.X - outer.X;
            res.Y = inner.Y - outer.Y;
            res.Width = inner.Width;
            res.Height = inner.Height;

            return res;
        }

        public static Rectangle ScaleRect(Rectangle r, double scale)
        {
            Rectangle res = new Rectangle();
            res.X = (int)(r.X * scale);
            res.Y = (int)(r.Y * scale);
            res.Width = (int)(r.Width * scale);
            res.Height= (int)(r.Height* scale);

            return res;
        }

        public static Rectangle RectangleFromRECT(RECT r)
        {
            Rectangle rect = new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
            return rect;
        }

        public static Screen GetScreenForRectLogical(Rectangle r)
        {
            Screen[] screens = Screen.AllScreens;

            foreach (Screen screen in screens)
            {
                //Does the item's top left position fit into the current window
                if ((r.Left >= screen.Bounds.Left) &&
                    (r.Left <= screen.Bounds.Right) &&
                    (r.Top >= screen.Bounds.Top) &&
                    (r.Top <= screen.Bounds.Bottom))
                {
                    return screen;
                }
            }

            return null;
        }


        const int ENUM_CURRENT_SETTINGS = -1;

        public static double GetScaleFactorForScreen(Screen screen)
        {
            LowLevelUtils.DEVMODE dm = new LowLevelUtils.DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(LowLevelUtils.DEVMODE));
            LowLevelUtils.EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref dm);

            return ((double)dm.dmPelsWidth / (double)screen.Bounds.Width);
        }

        public static Rectangle GetPhysicalRectangleFromScreen(Screen screen)
        {
            double scale = GetScaleFactorForScreen(screen);
            Rectangle r = new Rectangle(0, 0, (int)(screen.Bounds.Width * scale), (int)(screen.Bounds.Height * scale));
            return r;
        }

        public static List<DesktopWindow> GetDesktopWindows()
        {
            var collection = new List<DesktopWindow>();
            LowLevelUtils.EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                var result = new StringBuilder(255);
                LowLevelUtils.GetWindowText(hWnd, result, result.Capacity + 1);
                string title = result.ToString();

                var isVisible = !string.IsNullOrEmpty(title) && LowLevelUtils.IsWindowVisible(hWnd);

                collection.Add(new DesktopWindow { Handle = hWnd, Title = title, IsVisible = isVisible });

                return true;
            };

            LowLevelUtils.EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
            return collection;
        }

        public static Rectangle GetWindowRectLogical(IntPtr windowHandle)
        {
            RECT rect = new RECT();
            IntPtr error = LowLevelUtils.GetWindowRect(windowHandle, ref rect);
            return RectangleFromRECT(rect);
        }

    }
    public class DesktopWindow
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
