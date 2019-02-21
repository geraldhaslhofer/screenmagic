using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
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

    public class ProcessHelpers
    {

        public static IEnumerable<DesktopWindow> GetActiveWindows()
        {
            var windows = User32Helper.GetDesktopWindows();
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

public class User32Helper
    {
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction,
            IntPtr lParam);

        public static List<DesktopWindow> GetDesktopWindows()
        {
            var collection = new List<DesktopWindow>();
            EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                var result = new StringBuilder(255);
                GetWindowText(hWnd, result, result.Capacity + 1);
                string title = result.ToString();

                var isVisible = !string.IsNullOrEmpty(title) && IsWindowVisible(hWnd);

                collection.Add(new DesktopWindow { Handle = hWnd, Title = title, IsVisible = isVisible });

                return true;
            };

            EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
            return collection;
        }
    }
    
}
