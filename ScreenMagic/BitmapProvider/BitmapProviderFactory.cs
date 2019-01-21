using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class BitmapProviderFactory
    {

        public static IBitmapProvider GetBitmapProvider()
        {
            if (Config.IsTestBitmapProvider()) 
            {
                return new TestStubScreenshotProvider();
            }
            else
            {
                return new PrintScreenProvider();
            }
        }
    }
}

