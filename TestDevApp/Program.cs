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
            System.Diagnostics.Debug.WriteLine(System.Windows.Forms.Screen.AllScreens[0].Bounds);
            System.Diagnostics.Debug.WriteLine(Utils.GetScaleFactorForScreen(Screen.AllScreens[0]));
            while (true)
            {
                System.Diagnostics.Debug.WriteLine(System.Windows.Forms.Cursor.Position);

            }


            ////Figure out where Your Phone is
            //var YP = GlobalUtils.Utils.GetYourPhoneWindow();

            //Screen s;
            //Rectangle rPhysical;

            ////IntPtr handle, out Screen screen, out Rectangle windowAbsoluteLogical, out Rectangle windowRelativeLogical, out Rectangle windowRelativePhysical, out double scaleFactor

            //GlobalUtils.Utils.LocateProcessWindowRelativePhysical(YP.Handle, out s, out rPhysical);
            //GlobalUtils.Utils.CaptureScreenFromRectPhysical(s, rPhysical);

            ////var r = MonitorHelper.GetWindowRect(YP.Handle);




            ////ScreenGrab.CaptureAndSaveAllScreens();
        }
    }
}
