using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenMagic
{
    //Handles tagging mode by 
    // - saving the image to disk
    // - capturing rectangle semantics
    // - saving metadata to disk
    public class Tagger
    {

        
        private Bitmap _bmp;
        private List<Region> _taggedRegion = new List<Region>();
        string _pathToPersist;
        public Tagger(string pathToPersist)
        {
            _pathToPersist = pathToPersist;
        }

        public void SetBitmap(Bitmap bmp)
        {
            _bmp = bmp;
        }
        public void AddRegion(Region r)
        {
            _taggedRegion.Add(r);
        }

        public void Persist()
        {
            //Write bitmap first
            string picId = Guid.NewGuid().ToString();
            string imgPath = Path.Combine(_pathToPersist, picId + ".jpg");
            _bmp.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            //Write each identified rectangle
            string metaDataPath = Path.Combine(_pathToPersist, picId + ".csv");
            StreamWriter w = new StreamWriter(metaDataPath);
            foreach (var m in _taggedRegion)
            {
                w.WriteLine(m.SerializeToCsv());
            }
            w.Flush();
            w.Close();
        }

    }
    
}
