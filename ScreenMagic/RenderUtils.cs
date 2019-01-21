﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
//using System.Drawing;
using System.IO;

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
        public static void CopyRegionIntoImage(System.Drawing.Bitmap srcBitmap, System.Drawing.Rectangle srcRegion, ref System.Drawing.Bitmap destBitmap, System.Drawing.Rectangle destRegion)
        {
            using (System.Drawing.Graphics grD = System.Drawing.Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, System.Drawing.GraphicsUnit.Pixel);
            }
        }
        public static BitmapSource ScaleImage(BitmapSource bmp, double scale)
        {
            var target = new RenderTargetBitmap((int)((double)bmp.PixelWidth * scale), (int)((double)bmp.PixelHeight * scale), bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();
            using (var r = visual.RenderOpen())
            {
                r.DrawImage(bmp, new Rect(0, 0, (double)bmp.Width * scale,(double) bmp.Height * scale));
            }
            target.Render(visual);
            return target;

        }
        public static BitmapSource DrawOriginalBmps(BitmapSource bmp, OcrResults results)
        {

            // bmp is the original BitmapImage
            var target = new RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {
                //render background
                //BitmapSource bg = GetImageFromPath(Path.Combine(Utils.GetAssemblyPath(), "Assets\\background.jpg"));
                //r.DrawImage(bg, new Rect(0, 0, bmp.Width, bmp.Height));
                r.DrawImage(bmp, new Rect(0, 0, bmp.Width, bmp.Height));

                if (results != null && results.BoundingBoxes != null)
                //Render bounding boxes
                foreach (var box in results.BoundingBoxes)
                {
                    //r.DrawRectangle(new SolidColorBrush(Color.FromRgb(255,255,255)), new Pen(Brushes.LightGray, 4.0), new Rect(box.X, box.Y, box.Width, box.Height));
                    r.DrawRectangle(null, new Pen(Brushes.LightBlue, 1.0), new Rect(box.X, box.Y, box.Width, box.Height));

                }

                //foreach (var ocrResult in results.Results)
                //{
                //    if (true)
                //    {

                //        Int32Rect rec = new Int32Rect(ocrResult.X, ocrResult.Y, ocrResult.Width , ocrResult.Height );
                //        CroppedBitmap b = new CroppedBitmap(bmp, rec);

                //        r.DrawImage(b, new Rect(ocrResult.X, ocrResult.Y, ocrResult.Width, ocrResult.Height));
                //    }

                //    else
                //    {
                //        Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                //        FormattedText txt = new FormattedText(ocrResult.Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 8, Brushes.Black);
                //        r.DrawText(txt, new Point(ocrResult.X, ocrResult.Y));

                //    }
                //}
            }

            target.Render(visual);
            return target;

        }
        //public static BitmapSource DrawTextItems(BitmapSource bmp, OcrResults results)
        //{

        //    // bmp is the original BitmapImage
        //    var target = new RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
        //    var visual = new DrawingVisual();

        //    using (var r = visual.RenderOpen())
        //    {
        //        Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        //        foreach (var ocrResult in results.Results)
        //        {
        //            FormattedText txt = new FormattedText(ocrResult.Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 24, Brushes.Black);
        //            r.DrawText(txt, new Point(ocrResult.X, ocrResult.Y));       
        //        }
        //    }

        //    target.Render(visual);
        //    return target;
            
        //}
        //public static BitmapSource DrawBorder(BitmapSource bmp)
        //{
        //    // bmp is the original BitmapImage
        //    var target = new RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
        //    var visual = new DrawingVisual();

        //    using (var r = visual.RenderOpen())
        //    {
        //        r.DrawImage(bmp, new Rect(0, 0, bmp.Width, bmp.Height));
        //        //r.DrawLine(new Pen(Brushes.Red, 10.0), );
        //        r.DrawRectangle(null, new Pen(Brushes.Red, 4.0), new Rect(new Point(0, 0), new Point(bmp.Width, bmp.Height)));
                
        //        Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        //        FormattedText txt = new FormattedText("Test", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 30, Brushes.Black);
        //        r.DrawText(txt, new Point(10, 10));

        //    }

        //    target.Render(visual);
        //    return target;
        //}
    }
}
