using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalUtils;
using System.Windows.Forms;
using System.Drawing;

namespace TestDevApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Figure out where Your Phone is
            var YP = GlobalUtils.Utils.GetYourPhoneWindow();

            Screen s;
            Rectangle rPhysical;

            GlobalUtils.Utils.LocateProcessWindowRelativePhysical(YP.Handle, out s, out rPhysical);
            GlobalUtils.Utils.CaptureScreenFromRectPhysical(s, rPhysical);

            //var r = MonitorInfo.GetWindowRect(YP.Handle);




            //ScreenGrab.CaptureAndSaveAllScreens();
        }
    }
}
