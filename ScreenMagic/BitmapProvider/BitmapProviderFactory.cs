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
                //return new TestStubScreenshotProvider();
                throw new NotImplementedException("No test provider");
            }
            else
            {
                return new OptimizedScreenShotProvider();
            }
        }
    }
}

