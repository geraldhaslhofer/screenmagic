using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalUtils;
using System.Windows.Forms;
using System.Drawing;
using Utils;


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
            

            if (args.Count() > 2)
            {
                switch (args[0])
                {
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

                        } break;
                    default:
                        {
                            throw new NotSupportedException();
                        };
                }
            }
        }
    }
}
