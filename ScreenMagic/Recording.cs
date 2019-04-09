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

        public static void Retrieve(long id, out byte[] jpegEncodedPath, out OcrResults ocrResults)
        {
            string filenameImage = id.ToString() + ".jpeg";
            string filename = Path.Combine(GetWorkingDir(), filenameImage);
            FileStream f = new FileStream(filename, FileMode.Open);
            jpegEncodedPath = new byte[f.Length];
            f.Read(jpegEncodedPath, 0, (int) f.Length);
            f.Close();

            //OcrResults

            string serializePath = Path.Combine(GetWorkingDir(), id.ToString() + ".json");
            ocrResults = DeserializeOcrResults(Path.Combine(GetWorkingDir(), serializePath));


        }

      

        public static void Persist(byte[] jpegEncodedImage, OcrResults results, out long id)
        {   
            string prefix = GetUniqueFileNamePrefix();
            string filenameImage = prefix + ".jpeg";
            string filename = Path.Combine(GetWorkingDir(), filenameImage);

            //Persist jpg locally
            FileStream f = new FileStream(filename, FileMode.CreateNew);
            f.Write(jpegEncodedImage, 0, jpegEncodedImage.Count());
            f.Close();
            
            //Persist text locally
            string serializePath = Path.Combine(GetWorkingDir(), prefix + ".json");
            SerializeOcrResults(results, Path.Combine(serializePath));

            id = long.Parse(prefix);
        }

        public async static Task<RecordingResult> ProcessAndPersistScreenshot(Bitmap screen)
        {
            double scaleFactor = 1;
            byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);
            var results = await OcrUtils.GetOcrResults(jpegEncodedImage, scaleFactor);

            long id;
            
            Persist(jpegEncodedImage, results, out id);
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

