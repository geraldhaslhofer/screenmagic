using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
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

        
        private BitmapSource _bmp;
        private List<Region> _taggedRegion = new List<Region>();
        string _pathToPersist;
        public Tagger(string pathToPersist)
        {
            _pathToPersist = pathToPersist;
        }

        public void SetBitmap(BitmapSource bmp)
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
            using (var fileStream = new FileStream(imgPath, FileMode.Create))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(_bmp));
                encoder.Save(fileStream);
            }
            
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
        public List<Region> GetTaggedRegions()
        {
            return _taggedRegion;
        }
    }
    
}
