using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace ScreenMagic
{
    class Recording
    {
        private static string GetWorkingDir()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UniversalCopy");
        }

        private static string GetUniqueFileName()
        {
            return DateTime.Now.Ticks.ToString() + ".jpg";
        }

        public static void EnsureWorkingDirectory()
        {
            string workingDir = GetWorkingDir();
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
        }

        public async static Task<bool> CaptureAndPersistScreenshot(Bitmap screen)
        {
            //double scaleFactor = 1;
            byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);

            string filename = Path.Combine(GetWorkingDir(), GetUniqueFileName());
            FileStream f = new FileStream(filename, FileMode.CreateNew);
            f.Write(jpegEncodedImage, 0, jpegEncodedImage.Count());
            f.Close();

            //var results = await OcrUtils.GetOcrResults(jpegEncodedImage, scaleFactor);
            return true;
        }
    }

    }

