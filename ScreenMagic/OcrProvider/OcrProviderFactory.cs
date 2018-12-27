using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    class OcrProviderFactory
    {
        private static bool _isTest = true;

        public static IOcrResultProvider GetOcrResultsProvider()
        {
            if (_isTest)
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
