using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;

namespace ScreenMagic
{
    class Recording
    {
        private static string GetWorkingDir()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UniversalCopy");
        }


        private static string GetUniqueFileNamePrefix()
        {
            return DateTime.Now.Ticks.ToString();
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
            double scaleFactor = 1;
            byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);

            string prefix = GetUniqueFileNamePrefix();

            string filename = Path.Combine(GetWorkingDir(), prefix + ".jpeg");
            FileStream f = new FileStream(filename, FileMode.CreateNew);
            f.Write(jpegEncodedImage, 0, jpegEncodedImage.Count());
            f.Close();

            var results = await OcrUtils.GetOcrResults(jpegEncodedImage, scaleFactor);
            string serializePath = Path.Combine(GetWorkingDir(), prefix + ".json");

            SerializeOcrResults(results, Path.Combine(serializePath));

            var test = DeserializeOcrResults(serializePath);

            return true;
        }

        public static void SerializeOcrResults(OcrResults results, string path)
        {
            string value = JsonConvert.SerializeObject(results);
            StreamWriter w = new StreamWriter(path);
            w.Write(value);
            w.Flush();
            w.Close();
        }

        public static OcrResults DeserializeOcrResults(string path)
        {
            StreamReader r = new StreamReader(path);
            string json = r.ReadToEnd();
            r.Close();

            var result=  JsonConvert.DeserializeObject<OcrResults>(json);
            return result;
        }

    }

    }

