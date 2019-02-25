using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;


namespace ScreenMagic
{
    public static class ScreenExtensions
    {
        public static void GetDpi(this System.Windows.Forms.Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }

    enum TernaryRasterOperations : uint
    {
        /// <summary>dest = source</summary>
        SRCCOPY = 0x00CC0020,
        /// <summary>dest = source OR dest</summary>
        SRCPAINT = 0x00EE0086,
        /// <summary>dest = source AND dest</summary>
        SRCAND = 0x008800C6,
        /// <summary>dest = source XOR dest</summary>
        SRCINVERT = 0x00660046,
        /// <summary>dest = source AND (NOT dest)</summary>
        SRCERASE = 0x00440328,
        /// <summary>dest = (NOT source)</summary>
        NOTSRCCOPY = 0x00330008,
        /// <summary>dest = (NOT src) AND (NOT dest)</summary>
        NOTSRCERASE = 0x001100A6,
        /// <summary>dest = (source AND pattern)</summary>
        MERGECOPY = 0x00C000CA,
        /// <summary>dest = (NOT source) OR dest</summary>
        MERGEPAINT = 0x00BB0226,
        /// <summary>dest = pattern</summary>
        PATCOPY = 0x00F00021,
        /// <summary>dest = DPSnoo</summary>
        PATPAINT = 0x00FB0A09,
        /// <summary>dest = pattern XOR dest</summary>
        PATINVERT = 0x005A0049,
        /// <summary>dest = (NOT dest)</summary>
        DSTINVERT = 0x00550009,
        /// <summary>dest = BLACK</summary>
        BLACKNESS = 0x00000042,
        /// <summary>dest = WHITE</summary>
        WHITENESS = 0x00FF0062,
        /// <summary>
        /// Capture window as seen on screen.  This includes layered windows 
        /// such as WPF windows with AllowsTransparency="true"
        /// </summary>
        CAPTUREBLT = 0x40000000
    }
    public class LowLevelUtils
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern bool DeleteDC([In] IntPtr hdc);
        [DllImport("User32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("User32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDC(IntPtr lpszDriver, string lpszDevice,
         IntPtr lpszOutput, IntPtr lpInitData);

        [DllImport("GDI32.DLL", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXDest, int nYDest, int nDestWidth, int nDestHeight,
          IntPtr hdcSrc, int nXSrc, int nYSrc, int nSrcWidth, int nSrcHeight, Int32 dwRop);

        public static Bitmap GetBitmapFromHwnd(IntPtr hwnd)
        {
            //Get device context 
            var rect = Utils.GetWindowRect(hwnd);
            return GetDesktopWindowCaptureAsBitmap(rect);
        }

        private static void GetScaleFactorRange(out double minScale, out double maxScale)
        {
            Screen[] screens = Screen.AllScreens;

            minScale= double.MaxValue;
            maxScale= double.MinValue;
            foreach (Screen screen in screens)
            {
                double curScaleFactor = MonitorInfo.GetScaleFactorForScreen(screen);
                if (curScaleFactor< minScale) minScale= curScaleFactor;
                if (curScaleFactor> maxScale) maxScale= curScaleFactor;
            }
        }

        private static Screen GetScreenForRect(RECT r)
        {
            Screen[] screens = Screen.AllScreens;

            foreach (Screen screen in screens)
            {
                //Does the item's top left position fit into the current window
                if ((r.Left >= screen.Bounds.Left) && 
                    (r.Left<= screen.Bounds.Right) &&
                    (r.Top >= screen.Bounds.Top) &&
                    (r.Top<= screen.Bounds.Bottom))
                {
                    return screen;
                }
            }

            return null;
        }


        public static Bitmap GetDesktopWindowCaptureAsBitmap(RECT r)
        {
            Rectangle rcScreen = Rectangle.Empty;
            Screen[] screens = Screen.AllScreens;

            int rWidth = r.Right - r.Left;
            int rHeight = r.Bottom - r.Top;

            
            // Create a rectangle encompassing all screens...
            foreach (Screen screenA in screens)
                rcScreen = Rectangle.Union(rcScreen, screenA.Bounds);
            
            // Create a composite bitmap of the size of all screens...
            Bitmap finalBitmap = new Bitmap(rWidth, rHeight);

            // Get a graphics object for the composite bitmap and initialize it...
            Graphics g = Graphics.FromImage(finalBitmap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.FillRectangle(
               SystemBrushes.Desktop,
               0,
               0,
               rcScreen.Width - rcScreen.X,
               rcScreen.Height - rcScreen.Y);

            // Get an HDC for the composite area...
            IntPtr hdcDestination = g.GetHdc();

            // Now, loop through screens, 
            // Blting each to the composite HDC created above...

            Screen screen = GetScreenForRect(r);


            //DisplayInformation displayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

            
            double minScale;
            double maxScale;
            double curScale;
            
            //Is there a scale factor to factor in?
            GetScaleFactorRange(out minScale, out maxScale);
            curScale = MonitorInfo.GetScaleFactorForScreen(screen);

            double scaleFactor = maxScale / curScale;
            //current DPI
            


            // Create DC for each source monitor...
            IntPtr hdcSource = CreateDC(
                IntPtr.Zero,
                screen.DeviceName,
                IntPtr.Zero,
                IntPtr.Zero);

            // Blt the source directly to the composite destination...
            //int xDest = screen.Bounds.X - rcScreen.X;
            //int yDest = screen.Bounds.Y - rcScreen.Y;

            int xDest = 0;
            int yDest = 0;

           

            bool success = StretchBlt(
                hdcDestination,
                xDest,
                yDest,
                //screen.Bounds.Width,
                //screen.Bounds.Height,
                rWidth,
                rHeight,
                hdcSource,
                (int)((r.Left - screen.Bounds.Left)/scaleFactor ),
                (int)((r.Top - screen.Bounds.Top)/scaleFactor),
                (int)(rWidth / scaleFactor),
                (int)(rHeight / scaleFactor),
                (int)TernaryRasterOperations.SRCCOPY);

            //  System.Diagnostics.Trace.WriteLine(screen.Bounds);

            if (!success)
            {
                System.ComponentModel.Win32Exception win32Exception =
                    new System.ComponentModel.Win32Exception();
                System.Diagnostics.Trace.WriteLine(win32Exception);
            }

            // Cleanup source HDC...
            DeleteDC(hdcSource);


            // Cleanup destination HDC and Graphics...
            g.ReleaseHdc(hdcDestination);
            g.Dispose();

            // IntPtr hDC = GetDC(IntPtr.Zero);
            // Graphics gDest = Graphics.FromHdc(hDC);
            // gDest.DrawImage(finalBitmap, 0, 0, 640, 480);
            // gDest.Dispose();
            // ReleaseDC(IntPtr.Zero, hDC);

            // Return composite bitmap which will become our Form's PictureBox's image...
            return finalBitmap;
        }
    }
}
