//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestDevApp
//{
//    class ProcessUtils
//    {
//        public static IEnumerable<DesktopWindow> GetActiveWindows()
//        {
//            var windows = MonitorInfo.GetDesktopWindows();
//            var visible = from x in windows where x.IsVisible && x.Title.Length > 2 orderby x.Title select x;
//            return visible;
//        }

//        public static DesktopWindow GetYourPhoneWindow()
//        {
//            var windows = GetActiveWindows();
//            var yourPhone = from x in windows where x.Title.ToString().ToLower().Contains("your phone") select x;
//            if (yourPhone == null || yourPhone.Count() == 0) return null;
//            return yourPhone.First();
//        }
//    }
//}
