using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;


namespace GlobalUtils
{
    public class BitmapManipulation
    {
        public static BitmapSource DrawSelectionRectangle(BitmapSource src, System.Drawing.Rectangle rec)
        {

            
            // bmp is the original BitmapImage
            var target = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, src.DpiX, src.DpiY, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {
                //Draw image background
                r.DrawImage(src, new Rect(0, 0, src.Width, src.Height));
                r.DrawRectangle(null, new Pen(Brushes.Red, 1.0), new Rect(rec.X, rec.Y, rec.Width, rec.Height));
            }

            target.Render(visual);
            return target;
        }

    }
}
