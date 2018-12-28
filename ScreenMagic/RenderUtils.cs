﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ScreenMagic
{
    class RenderUtils
    {
        private static BitmapSource GetImageFromPath(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();

            return bitmap;
        }
        public static BitmapSource DrawTextItems(BitmapSource bmp, OcrResults results)
        {

            // bmp is the original BitmapImage
            var target = new RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {
                Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

                foreach (var ocrResult in results.Results)
                {
                    FormattedText txt = new FormattedText(ocrResult.Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 24, Brushes.Black);
                    r.DrawText(txt, new Point(ocrResult.X, ocrResult.Y));       
                }
            }

            target.Render(visual);
            return target;
            
        }
        public static BitmapSource DrawBorder(BitmapSource bmp)
        {
            // bmp is the original BitmapImage
            var target = new RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {
                r.DrawImage(bmp, new Rect(0, 0, bmp.Width, bmp.Height));
                //r.DrawLine(new Pen(Brushes.Red, 10.0), );
                r.DrawRectangle(null, new Pen(Brushes.Red, 4.0), new Rect(new Point(0, 0), new Point(bmp.Width, bmp.Height)));
                
                Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                FormattedText txt = new FormattedText("Test", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 30, Brushes.Black);
                r.DrawText(txt, new Point(10, 10));

            }

            target.Render(visual);
            return target;
        }
    }
}
