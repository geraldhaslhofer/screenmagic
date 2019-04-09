//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Drawing;

//namespace ScreenMagic
//{
//    class CachedBitmapProvider : IBitmapProvider
//    {
//        private long _id;
//        public CachedBitmapProvider(long id)
//        {
//            _id = id;
//        }
//        Bitmap IBitmapProvider.CaptureScreenshot()
//        {
//            byte[] jpeg;
//            OcrResults r;
//            Recording.Retrieve(_id, out jpeg, out r);


//            System.IO.MemoryStream m = new System.IO.MemoryStream(jpeg);
//            var img = Image.FromStream(m);

//            return new Bitmap(img);


//        }
//    }
//}
