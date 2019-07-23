using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalUtils;
using System.Windows.Forms;
using System.Drawing;
using Utils;
using System.IO;
using System.Diagnostics;
using System.Linq.Expressions;

namespace TestDevApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Main2(args).Wait();
        }

        static async Task Main2(string[] args)
        {

            try
            {


                if (args.Count() > 2)
                {
                    switch (args[0])
                    {
                        case "-batch":
                            {
                                string path = args[1];
                                string destDir = args[2];
                                string[] jpegs = System.IO.Directory.GetFiles(path);
                                foreach (string jpeg in jpegs)
                                {
                                    string filename = Path.GetFileNameWithoutExtension(jpeg);
                                    string json = Path.Combine(destDir, filename + ".json");
                                    string rendered = Path.Combine(destDir, filename + ".jpeg");
                                    string combined = Path.Combine(destDir, filename + "_comb" + ".jpeg");

                                    var regions = await OcrProcessor.GetRegions(jpeg);
                                    PixGenerator.GeneratePix(regions, rendered);

                                    Combiner.CombineAndNormalize(jpeg, rendered, combined);
                                }

                            }
                            break;


                        case "-ocr":
                            {
                                //param 1: source jpeg path
                                //param 2: regions .json path

                                await OcrProcessor.OcrPicture(args[1], args[2]);


                            }; break;
                        case "-render":
                            {

                                //param 1: regions .json path
                                //param 1: output jpeg
                                PixGenerator.GeneratePix(args[1], args[2]);

                            }
                            break;

                        
                        default:
                            {
                                throw new NotSupportedException();
                            };
                    }

                    
                }

            }  catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
