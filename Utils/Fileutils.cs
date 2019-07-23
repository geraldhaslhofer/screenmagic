using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Utils
{
    public class Fileutils
    {
        public static void SerializeBitmapSource(BitmapSource target, string destinationPath)
        {
            var fileStream = new FileStream(destinationPath, FileMode.Create);
            
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(target));
            encoder.Save(fileStream);

            fileStream.Flush();
            fileStream.Close();

        }

        public static BitmapSource LoadBitmapSourceFromFile(string path)
        {
            FileStream fileStream =
    new FileStream(path, FileMode.Open, FileAccess.Read);

            var img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = fileStream;
            img.EndInit();

            fileStream.Close();
            return img;
        }

        public static Bitmap DeserializeJpeg(Stream s)
        {
            return (Bitmap)Bitmap.FromStream(s);

        }

        public static Bitmap DeserializeJpeg(string path)
        {
            return (Bitmap)Bitmap.FromFile(path);
           
        }
        public static byte[] GetFileFromPath(string path)
        {
            FileStream f = new FileStream(path, FileMode.Open);
            byte[] content = new byte[f.Length];
            f.Read(content, 0, (int)f.Length);
            f.Close();
            return content;
        }

        public static void SerializeSemanticRegions(SemanticRegions results, string path)
        {
            string value = JsonConvert.SerializeObject(results);
            StreamWriter w = new StreamWriter(path);
            w.Write(value);
            w.Flush();
            w.Close();
        }
        public static SemanticRegions DeserializeSemanticRegions(string path)
        {
            StreamReader r = new StreamReader(path);
            string json = r.ReadToEnd();
            r.Close();

            var result = JsonConvert.DeserializeObject<SemanticRegions>(json);
            return result;
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

            var result = JsonConvert.DeserializeObject<OcrResults>(json);
            return result;
        }

    }
}
