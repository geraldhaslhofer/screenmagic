//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Drawing;
//using System.Runtime.InteropServices;

//namespace TestDevApp
//{
//    class MonitorInfo
//    {
//        const int ENUM_CURRENT_SETTINGS = -1;

//        public static double GetScaleFactorForScreen(Screen screen)
//        {
//            LowLevelUtils.DEVMODE dm = new LowLevelUtils.DEVMODE();
//            dm.dmSize = (short)Marshal.SizeOf(typeof(LowLevelUtils.DEVMODE));
//            LowLevelUtils.EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref dm);

//            return ((double)dm.dmPelsWidth / (double)screen.Bounds.Width);
//        }

//        public static Rectangle GetPhysicalRectangleFromScreen(Screen screen)
//        {
//            double scale = GetScaleFactorForScreen(screen);
//            Rectangle r = new Rectangle(0, 0, (int)(screen.Bounds.Width * scale), (int)(screen.Bounds.Height * scale));
//            return r;
//        }

//        public static List<DesktopWindow> GetDesktopWindows()
//        {
//            var collection = new List<DesktopWindow>();
//            LowLevelUtils.EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
//            {
//                var result = new StringBuilder(255);
//                LowLevelUtils.GetWindowText(hWnd, result, result.Capacity + 1);
//                string title = result.ToString();

//                var isVisible = !string.IsNullOrEmpty(title) && LowLevelUtils.IsWindowVisible(hWnd);

//                collection.Add(new DesktopWindow { Handle = hWnd, Title = title, IsVisible = isVisible });

//                return true;
//            };

//            LowLevelUtils.EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
//            return collection;
//        }

//        public static RECT GetWindowRect(IntPtr windowHandle)
//        {
//            RECT rect = new RECT();
//            IntPtr error = LowLevelUtils.GetWindowRect(windowHandle, ref rect);
//            return rect;
//        }

//    }
//    public class DesktopWindow
//    {
//        public IntPtr Handle { get; set; }
//        public string Title { get; set; }
//        public bool IsVisible { get; set; }

//        public override string ToString()
//        {
//            return Title;
//        }
//    }
//}
