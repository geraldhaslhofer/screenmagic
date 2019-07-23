using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

using Utils;
using System.Windows;

namespace TestDevApp
{
    public class PixGenerator
    {
        public static void GeneratePix(string regionsFile, string destinationPath)
        {
            var regions = Fileutils.DeserializeSemanticRegions(regionsFile);
            GeneratePix(regions, destinationPath);
        }

        public static void GeneratePix(SemanticRegions regions, string destinationPath)
        {
            BoundingBox b = regions.GetBorder();

            BitmapSource s = new BitmapImage();
            var target = new RenderTargetBitmap(b.Width, b.Height, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {


                foreach (var aRegion in regions.GetNonBorder())
                //Draw image background
                {
                    r.DrawRectangle(Brushes.Red, new Pen(Brushes.Red, 1.0), new Rect(aRegion.Box.X, aRegion.Box.Y, aRegion.Box.Width, aRegion.Box.Height));
                    //r.DrawRectangle(null, new Pen(Brushes.Red, 1.0), new Rect(aRegion.Box.X, aRegion.Box.Y, aRegion.Box.Width, aRegion.Box.Height));
                }
            }

            target.Render(visual);


            //Remove
            //{
            //    var tmp = Combiner.CombineAndNormalizeBitmap(target, target);
            //    Fileutils.SerializeBitmapSource(tmp, @"c:\data\test\foo.jpg");

            //}

            Fileutils.SerializeBitmapSource(target, destinationPath);


        }
        //Generate a jpeg based on semanticRegionInfo
        public static void _Test(SemanticRegions regions, string destinationPath)
        {
            BoundingBox b = regions.GetBorder();

            BitmapSource s = new BitmapImage();
            var target = new RenderTargetBitmap(b.Width, b.Height, 144, 144, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {
                //Draw image background
                
                r.DrawRectangle(null, new Pen(Brushes.Red, 1.0), new Rect(30, 30, 30, 30));
            }

            target.Render(visual);

            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(target));
                encoder.Save(fileStream);
            }
        }
    }
}
