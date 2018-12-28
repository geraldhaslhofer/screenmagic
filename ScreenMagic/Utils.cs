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

namespace ScreenMagic
{
    class Utils
    {
        public static byte[] SerializeBitmapToJpeg(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();

        }
        public static IntPtr Activate()
        {
            var prc = Process.GetProcessesByName("outlook");
            if (prc.Length > 0)
            {
                //(prc[0].MainWindowHandle);
                IntPtr handle = prc[0].MainWindowHandle;
                SetForegroundWindow(handle);
                //ShowWindow(handle, 1);
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

    }
}
