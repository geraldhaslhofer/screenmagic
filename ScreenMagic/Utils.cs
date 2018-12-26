using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace ScreenMagic
{
    class Utils
    {
        public static IntPtr Activate()
        {
            var prc = Process.GetProcessesByName("outlook");
            if (prc.Length > 0)
            {
                //SetForegroundWindow(prc[0].MainWindowHandle);
                IntPtr handle = prc[0].MainWindowHandle;
                ShowWindow(handle, 1);
                return handle;
                
            }
            return IntPtr.Zero;
        }
        public static Bitmap CaptureScreenshot(IntPtr windowHandle)
        {
            RECT rect = new RECT();
            IntPtr error = GetWindowRect(windowHandle, ref rect);

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(bmp).CopyFromScreen(rect.Left,
                                                   rect.Top,
                                                   0,
                                                   0,
                                                   new Size(width, height),
                                                   CopyPixelOperation.SourceCopy);

            return bmp;
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //[DllImport("user32.dll")]
        //public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

    }
}
