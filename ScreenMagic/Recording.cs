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
    public class RecordingResult
    {
        public long Id;
        public OcrResults Results;
        public Bitmap BitmapImage;
        public byte[] JpegImage;
    }
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

        public static long Persist(byte[] jpegEncodedImage, OcrResults results)
        {   
            string prefix = GetUniqueFileNamePrefix();
            string filename = Path.Combine(GetWorkingDir(), prefix + ".jpeg");
            FileStream f = new FileStream(filename, FileMode.CreateNew);
            f.Write(jpegEncodedImage, 0, jpegEncodedImage.Count());
            f.Close();
            string serializePath = Path.Combine(GetWorkingDir(), prefix + ".json");
            SerializeOcrResults(results, Path.Combine(serializePath));
            return long.Parse(prefix);
        }

        public async static Task<RecordingResult> ProcessAndPersistScreenshot(Bitmap screen)
        {
            double scaleFactor = 1;
            byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);
            var results = await OcrUtils.GetOcrResults(jpegEncodedImage, scaleFactor);
            long id = Persist(jpegEncodedImage, results);
            RecordingResult r = new RecordingResult();
            r.Id = id;
            r.JpegImage = jpegEncodedImage;
            r.Results = results;
            return r;
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

