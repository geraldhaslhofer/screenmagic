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
using System.Runtime.CompilerServices;

namespace TestDevApp
{
    class ProcessBatch
    { 
        public ProcessBatch(int size, string dir)
        {
            BatchCount = size;
            DestinationDir = dir;
        }
    
        public int BatchCount { get; set; }
        public string DestinationDir { get; set; }

    }

   


    class Program
    {
        static void Main(string[] args)
        {
            Main2(args).Wait();
        }
        private static Random rng = new Random();

        private static void Shuffle(List<string> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
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

                                List<ProcessBatch> batches = new List<ProcessBatch>();
                                batches.Add(new ProcessBatch(106, "test"));
                                batches.Add(new ProcessBatch(400, "train"));
                                batches.Add(new ProcessBatch(100, "val"));

                                string[] jpegs = System.IO.Directory.GetFiles(path);

                                var jpegsList = jpegs.ToList();
                                Shuffle(jpegsList);

                                int listCounter = 0;

                                foreach (var aBatch in batches)
                                {
                                    for (int count = 1; count <= aBatch.BatchCount; count++)
                                    {
                                        string jpeg = jpegsList[listCounter];
                                        listCounter++;

                                        string filename = Path.GetFileNameWithoutExtension(jpeg);
                                        string json = Path.Combine(destDir, filename + ".json");
                                        string rendered = Path.Combine(destDir, filename + ".jpeg");
                                        string combined = Path.Combine(destDir + @"\" + aBatch.DestinationDir, count.ToString() + ".jpg");

                                        var regions = await OcrProcessor.GetRegions(jpeg);
                                        PixGenerator.GeneratePix(regions, rendered);

                                        Combiner.CombineAndNormalize(jpeg, rendered, combined);

                                    }
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
