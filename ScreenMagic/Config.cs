using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class Config
    {
        public static IntPtr WindowToWatch;
        public static bool IsTestBitmapProvider()
        {
            return false;
        }
        public static bool IsTestOcrProvider()
        {
            return false;
        }
    }

   
}
