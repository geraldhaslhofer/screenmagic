using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class OcrProviderFactory
    {
        public static IOcrResultProvider GetOcrResultsProvider()
        {
            if (Config.IsTestOcrProvider())
            {
                return new TestStubOcrProvider();
            }
            else
            {
                return new CognitiveVisionOcrProvider();
            }
        }
    }
}
