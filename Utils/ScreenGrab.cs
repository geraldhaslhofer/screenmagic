using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;


namespace GlobalUtils
{
    class ScreenGrab
    {
        public static void CaptureAndSaveAllScreens()
        {
            Screen[] screens = Screen.AllScreens;

            foreach (Screen x in screens)
            {
                double scale = MonitorHelper.GetScaleFactorForScreen(x);
                CaptureScreen(x, scale);
            }
            
        }

        public static Bitmap CaptureScreenFromRectPhysical(Screen screen, Rectangle rPhysical)
        {
            //"Screen" does not show the physical size, but rather the logical size

            Rectangle destRect = MonitorHelper.GetPhysicalRectangleFromScreen(screen);

            //Create destionation bitmap
            Bitmap finalBitmap = new Bitmap(rPhysical.Width, rPhysical.Height);

            // Get a graphics object for the composite bitmap and initialize it...
            Graphics g = Graphics.FromImage(finalBitmap);
            IntPtr hdcDestination = g.GetHdc();
            
            IntPtr hdcSource = LowLevelUtils.CreateDC(
                IntPtr.Zero,
                screen.DeviceName,
                IntPtr.Zero,
                IntPtr.Zero);

            bool success = LowLevelUtils.StretchBlt(
                hdcDestination,
                0,
                0,
                rPhysical.Width,
                rPhysical.Height,
                hdcSource,
                rPhysical.X, rPhysical.Y, rPhysical.Width, rPhysical.Height,
                (int)TernaryRasterOperations.SRCCOPY);

            LowLevelUtils.DeleteDC(hdcSource);

            // Cleanup destination HDC and Graphics...
            g.ReleaseHdc(hdcDestination);
            g.Dispose();

            finalBitmap.Save(@"C:\Users\gerhas\Desktop\test\" + Guid.NewGuid().ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            return finalBitmap;
        }

        public static Bitmap CaptureScreen(Screen screen, double scaleFactor)
        {
            //"Screen" does not show the physical size, but rather the logical size
            
            Rectangle destRect = MonitorHelper.GetPhysicalRectangleFromScreen(screen);

            //Create destionation bitmap
            Bitmap finalBitmap = new Bitmap(destRect.Width , destRect.Height);

            // Get a graphics object for the composite bitmap and initialize it...
            Graphics g = Graphics.FromImage(finalBitmap);
            IntPtr hdcDestination = g.GetHdc();
            
            
            
            // Create a rectangle encompassing all screens...
            
            IntPtr hdcSource = LowLevelUtils.CreateDC(
                IntPtr.Zero,
                screen.DeviceName,
                IntPtr.Zero,
                IntPtr.Zero);

            bool success = LowLevelUtils.StretchBlt(
                hdcDestination,
                0,
                0,
                destRect.Width,
                destRect.Height,
                hdcSource, 
                0, 0, destRect.Width, destRect.Height,
                (int)TernaryRasterOperations.SRCCOPY);

            LowLevelUtils.DeleteDC(hdcSource);
            
            // Cleanup destination HDC and Graphics...
            g.ReleaseHdc(hdcDestination);
            g.Dispose();

            //finalBitmap.Save(@"C:\Users\gerhas\Desktop\test\" + Guid.NewGuid().ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            return finalBitmap;
            }

            //// Create a composite bitmap of the size of all screens...
            //Bitmap finalBitmap = new Bitmap(rWidth, rHeight);

            //// Get a graphics object for the composite bitmap and initialize it...
            //Graphics g = Graphics.FromImage(finalBitmap);
            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            //g.FillRectangle(
            //   SystemBrushes.Desktop,
            //   0,
            //   0,
            //   rcScreen.Width - rcScreen.X,
            //   rcScreen.Height - rcScreen.Y);

            //// Get an HDC for the composite area...
            //IntPtr hdcDestination = g.GetHdc();

            //// Now, loop through screens, 
            //// Blting each to the composite HDC created above...

            //Screen screen = GetScreenForRect(r);


            ////DisplayInformation displayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();


            //double minScale;
            //double maxScale;
            //double curScale;

            ////Is there a scale factor to factor in?
            //GetScaleFactorRange(out minScale, out maxScale);
            //curScale = MonitorHelper.GetScaleFactorForScreen(screen);

            //double scaleFactor = maxScale / curScale;
            ////current DPI



            //// Create DC for each source monitor...
            //IntPtr hdcSource = CreateDC(
            //    IntPtr.Zero,
            //    screen.DeviceName,
            //    IntPtr.Zero,
            //    IntPtr.Zero);

            //// Blt the source directly to the composite destination...
            ////int xDest = screen.Bounds.X - rcScreen.X;
            ////int yDest = screen.Bounds.Y - rcScreen.Y;

            //int xDest = 0;
            //int yDest = 0;



            //bool success = StretchBlt(
            //    hdcDestination,
            //    xDest,
            //    yDest,
            //    //screen.Bounds.Width,
            //    //screen.Bounds.Height,
            //    rWidth,
            //    rHeight,
            //    hdcSource,
            //    (int)((r.Left - screen.Bounds.Left) / scaleFactor),
            //    (int)((r.Top - screen.Bounds.Top) / scaleFactor),
            //    (int)(rWidth / scaleFactor),
            //    (int)(rHeight / scaleFactor),
            //    (int)TernaryRasterOperations.SRCCOPY);

            ////  System.Diagnostics.Trace.WriteLine(screen.Bounds);

            //if (!success)
            //{
            //    System.ComponentModel.Win32Exception win32Exception =
            //        new System.ComponentModel.Win32Exception();
            //    System.Diagnostics.Trace.WriteLine(win32Exception);
            //}

            //// Cleanup source HDC...
            //DeleteDC(hdcSource);


            //// Cleanup destination HDC and Graphics...
            //g.ReleaseHdc(hdcDestination);
            //g.Dispose();

            //// IntPtr hDC = GetDC(IntPtr.Zero);
            //// Graphics gDest = Graphics.FromHdc(hDC);
            //// gDest.DrawImage(finalBitmap, 0, 0, 640, 480);
            //// gDest.Dispose();
            //// ReleaseDC(IntPtr.Zero, hDC);

            //// Return composite bitmap which will become our Form's PictureBox's image...
            //return finalBitmap;
        
    }
}
