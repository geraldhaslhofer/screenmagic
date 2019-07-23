using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Globalization;
using Utils;

namespace TestDevApp
{
    class Combiner
    {
        private static double TRAINING_SIZE = 256;
        private static double DPI = 96;

        

        public static void CombineAndNormalize(string pic1, string pic2, string destinationPath)
        {
            BitmapSource src1 = Fileutils.LoadBitmapSourceFromFile(pic1);
            BitmapSource src2 = Fileutils.LoadBitmapSourceFromFile(pic2);

            var combined = CombineAndNormalizeBitmap(src1, src2);
            Fileutils.SerializeBitmapSource(combined, destinationPath);

        }

        private static BitmapSource NormalizeBitmap(BitmapSource src)
        {
            var scaled1 = new TransformedBitmap(src, new ScaleTransform(
                                                        TRAINING_SIZE / src.PixelWidth,
                                                        TRAINING_SIZE / src.PixelHeight));

            return scaled1;

        }
        public static BitmapSource CombineAndNormalizeBitmap(BitmapSource src1, BitmapSource src2)
        {

            var scaled1 = NormalizeBitmap(src1);
            var scaled2 = NormalizeBitmap(src2);

            // bmp is the original BitmapImage
            var target = new RenderTargetBitmap((int)TRAINING_SIZE * 2, (int)TRAINING_SIZE, DPI,DPI, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {
                //Draw first image
                r.DrawImage(scaled1, new Rect(0, 0, TRAINING_SIZE, TRAINING_SIZE)); //left
                r.DrawImage(scaled2, new Rect(TRAINING_SIZE, 0, TRAINING_SIZE, TRAINING_SIZE));  //right
                
            }

            target.Render(visual);
            return target;

        }
    }
    
}
