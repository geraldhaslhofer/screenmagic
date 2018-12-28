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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenMagic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr _windowToWatch = IntPtr.Zero;
        private System.Timers.Timer _timer;

        IBitmapProvider _bitmapProvider;
        IOcrResultProvider _ocrProvider;
        public MainWindow()
        {
            InitializeComponent();
            _bitmapProvider = BitmapProviderFactory.GetBitmapProvider();
            _ocrProvider = OcrProviderFactory.GetOcrResultsProvider();

            _timer = new System.Timers.Timer(10000);
            _timer.Elapsed += _timer_Elapsed;
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {    
            Utils.Activate();
            System.Threading.Thread.Sleep(2000);
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(() => this.Update()));
        }

        private async void Update( )
        {
            var screen = _bitmapProvider.CaptureScreenshot();
            var imageSource = Utils.ImageSourceForBitmap(screen);
            byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);
            
            var results = await OcrUtils.GetOcrResults(jpegEncodedImage);

            MainImage.Source = RenderUtils.DrawOriginalBmps(imageSource, results);
        }

        private void OCR_Click(object sender, RoutedEventArgs e)
        {
            //var result = OcrUtils.CatpureImage(@"C:\Users\gerhas\Desktop\tes.jpg");
        }
    }
}
