using System;
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
using Microsoft.Win32;

namespace ScreenMagic
{
    class Utils
    {
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public static IntPtr GetMainWindowsHandle()
        {
            Process cur = Process.GetCurrentProcess();
            cur.Refresh();
            return cur.MainWindowHandle;
        }
        public static double GetScale(Visual v)
        {
            var dpi = System.Windows.Media.VisualTreeHelper.GetDpi(v);
            return dpi.PixelsPerDip;
        }
        public static void ChangePos(IntPtr handle, RECT r)
        {
           
            SetWindowPos(handle, HWND_TOP, r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top, 0);
        }

        public static byte[] SerializeBitmapToJpeg(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();

        }

        public static IntPtr GetWindowHandleFromApp(string app)
        {
            var prc = Process.GetProcessesByName(app);
            if (prc.Length > 0)
            {
                return prc[0].MainWindowHandle;

            }
            return IntPtr.Zero;
        }

        public static void Activate()
        {
            IntPtr handle = Config.WindowToWatch;
            if (handle != IntPtr.Zero)
            {
                SetForegroundWindow(handle);
            }
        }

        public static RECT GetWindowRect(IntPtr windowHandle)
        {
            RECT rect = new RECT();
            IntPtr error = GetWindowRect(windowHandle, ref rect);
            return rect;
        }

        
        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc = new RECT();
            GetWindowRect(hwnd, ref rc);

            Bitmap bmp = new Bitmap(rc.Right - rc.Left, rc.Bottom- rc.Top, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(hwnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }
        public static Bitmap CaptureScreenshot(IntPtr windowHandle)
        {
            RECT rect = new RECT();
            IntPtr error = GetWindowRect(windowHandle, ref rect);

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics.FromImage(bmp).CopyFromScreen(rect.Left,
                                                   rect.Top,
                                                   0,
                                                   0,
                                                   new System.Drawing.Size(width, height),
                                                   CopyPixelOperation.SourceCopy);

            return bmp;
        }

        public static BitmapSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static string GetAssemblyPath()
        {
            string execAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string basePath = System.IO.Path.GetDirectoryName(execAssembly);
            return basePath;

        }

        public static void SetClipboardText(string text)
        {
            Thread thread = new Thread(() => Clipboard.SetText(text));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

         
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    }
}
