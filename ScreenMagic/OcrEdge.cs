﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Ocr;
using Windows.Globalization;

namespace ScreenMagic
{
    class OcrEdge
    {
        public static void DoSomething()
        {
            Language ocrLanguage = new Language("en");
            OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(ocrLanguage);

        }
    }
}
