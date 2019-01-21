using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenMagic
{
    /// <summary>
    /// Interaction logic for Screenshot.xaml
    /// </summary>
    public partial class Screenshot : Window
    {

        MainWindow _mainWindow = null;
        public Screenshot(MainWindow ownerWindow)
        {
            InitializeComponent();
            _mainWindow = ownerWindow;

            MainImage.MouseDown += MainImage_MouseDown; ;

        }

      
        public void SetImage(ImageSource img)
        {
            MainImage.Source = img;
        }

        private void MainImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickPos = e.GetPosition(MainImage);

            //Figure out position
            ImageSource imageSource = MainImage.Source;
            RenderTargetBitmap bitmapImage = (RenderTargetBitmap)imageSource;
            var pixelMousePositionX = e.GetPosition(MainImage).X * bitmapImage.PixelWidth / MainImage.Width;
            var pixelMousePositionY = e.GetPosition(MainImage).Y * bitmapImage.PixelHeight / MainImage.Height;

            System.Diagnostics.Debug.WriteLine(clickPos.ToString() + " " + pixelMousePositionX.ToString() + " " + pixelMousePositionY.ToString());

            double finalClickPosX = clickPos.X * _mainWindow._scale;
            double finalClickPosY = clickPos.Y * _mainWindow._scale;

            var textResults = _mainWindow._lastOcrResults.GetOcrResultFromCoordinate((int)finalClickPosX, (int)finalClickPosY);

            string textToCopy = string.Empty;

            if (textResults == null)
            {
                // try outer bounding box

                var smallestBox = _mainWindow._lastOcrResults.GetSmallestBoundingBox((int)clickPos.X, (int)clickPos.Y);
                if (smallestBox != null)
                {
                    var allOcrResults = _mainWindow._lastOcrResults.GetOcrResultFromBoundingbox(smallestBox);
                    if (allOcrResults != null && allOcrResults.Count > 0)
                    {
                        StringBuilder b = new StringBuilder();
                        foreach (var aRes in allOcrResults)
                        {
                            b.Append(aRes.Text);
                            b.Append(" ");
                        }
                        textToCopy = b.ToString();
                    }
                }

            }
            else
            {
                textToCopy = textResults.Text;
            }

            _mainWindow.SetCopyText(textToCopy);
            Utils.SetClipboardText(textToCopy);
            System.Diagnostics.Debug.WriteLine(textToCopy);
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
