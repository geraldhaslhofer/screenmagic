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
    enum AppVisualState
    {
        Launch = 0,
        WithScreenshot = 1,
        Companion = 2,
        Minimized = 3
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr _windowToWatch = IntPtr.Zero;
        private System.Timers.Timer _timer;
        private bool _isActive = false;
       
        OcrResults _lastOcrResults = null;

        
        IBitmapProvider _bitmapProvider;
        IOcrResultProvider _ocrProvider;
        public MainWindow()
        {
            InitializeComponent();
            _bitmapProvider = BitmapProviderFactory.GetBitmapProvider();
            _ocrProvider = OcrProviderFactory.GetOcrResultsProvider();

            _timer = new System.Timers.Timer(10000);
            _timer.Elapsed += _timer_Elapsed;
            this.SizeChanged += MainWindow_SizeChanged;
            this.StateChanged += MainWindow_StateChanged;
            MainImage.MouseDown += MainImage_MouseDown;
            ScaleSelection.Items.Add(100);
            ScaleSelection.Items.Add(66);
            ScaleSelection.Items.Add(50);
            ScaleSelection.Items.Add(25);
            ScaleSelection.Items.Add(10);
            ScaleSelection.SelectedIndex = 0;

            PopulateListOfApps();
            AppSelection.SelectionChanged += AppSelection_SelectionChanged;
            //this.Closing += MainWindow_Closing;
            
            SetWindowState(AppVisualState.Launch);
        }

        private void SetWindowState(AppVisualState state)
        {
            switch (state) {
                case AppVisualState.Minimized:
                    {
                        this.WindowState = WindowState.Minimized;
                        SetSmallState();
                        AppSelection.Visibility = Visibility.Visible;
                        ScaleSelection.Visibility = Visibility.Visible;
                        Execute.Visibility = Visibility.Visible;
                        MainImage.Visibility = Visibility.Collapsed;
                        Exit.Visibility = Visibility.Collapsed;
                        CopyText.Visibility = Visibility.Collapsed;

                    }
                    break;
                case AppVisualState.Launch:
                    {
                        SetSmallState();
                        AppSelection.Visibility = Visibility.Visible;
                        ScaleSelection.Visibility = Visibility.Visible;
                        Execute.Visibility = Visibility.Visible;
                        MainImage.Visibility = Visibility.Collapsed;
                        Exit.Visibility = Visibility.Collapsed;
                        CopyText.Visibility = Visibility.Collapsed;
                    } break;
                case AppVisualState.WithScreenshot:
                    {
                        //... move over app to watch
                        RECT r = Utils.GetWindowRect(Modes.WindowToWatch);
                        r.Bottom += 90; //Accommodate buttons etc.
                        Utils.ChangePos(Utils.GetMainWindowsHandle(), r);

                        AppSelection.Visibility = Visibility.Collapsed;
                        ScaleSelection.Visibility = Visibility.Collapsed;
                        Execute.Visibility = Visibility.Collapsed;
                        MainImage.Visibility = Visibility.Visible;
                        Exit.Visibility = Visibility.Visible;
                        CopyText.Visibility = Visibility.Visible;

                    } break;
                case AppVisualState.Companion:
                    {
                        SetSmallState();
                        AppSelection.Visibility = Visibility.Visible;
                        ScaleSelection.Visibility = Visibility.Visible;
                        Execute.Visibility = Visibility.Visible;
                        MainImage.Visibility = Visibility.Collapsed;
                        Exit.Visibility = Visibility.Collapsed;
                        CopyText.Visibility = Visibility.Collapsed;

                        if (Modes.WindowToWatch != IntPtr.Zero)
                        {
                            //Move out of the way     
                            RECT toWatch = Utils.GetWindowRect(Modes.WindowToWatch);
                            this.Left += (toWatch.Right - toWatch.Left);
                        }
                    } break;
            }
        }

        private void SetSmallState()
        {
            this.Width = 449;
            this.Height = 83;
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            SetWindowState(AppVisualState.Minimized);
        }

        private void AppSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DesktopWindow w = (DesktopWindow)AppSelection.SelectedItem;
            Modes.WindowToWatch = w.Handle;
        }

        private void PopulateListOfApps()
        {
            AppSelection.Items.Clear();

            var windows = User32Helper.GetDesktopWindows();
            var visible = from x in windows where x.IsVisible && x.Title.Length > 2 orderby x.Title select x;
            foreach (var avisible in visible)
            {
                AppSelection.Items.Add(avisible);
            }

        }
        private void MainImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickPos = e.GetPosition(MainImage);

            //Figure out position
            ImageSource imageSource = MainImage.Source;
            RenderTargetBitmap bitmapImage = (RenderTargetBitmap)imageSource;
            var pixelMousePositionX = e.GetPosition(MainImage).X * bitmapImage.PixelWidth / MainImage.Width;
            var pixelMousePositionY = e.GetPosition(MainImage).Y * bitmapImage.PixelHeight / MainImage.Height;

            System.Diagnostics.Debug.WriteLine(clickPos.ToString() + " " + pixelMousePositionX.ToString() + " "+ pixelMousePositionY.ToString());

            
            var textResults = _lastOcrResults.GetOcrResultFromCoordinate((int)clickPos.X, (int)clickPos.Y);

            string textToCopy = string.Empty;

            if (textResults == null)
            {
                // try outer bounding box

                var smallestBox  = _lastOcrResults.GetSmallestBoundingBox((int)clickPos.X, (int)clickPos.Y);
                if (smallestBox != null)
                {
                    var allOcrResults = _lastOcrResults.GetOcrResultFromBoundingbox(smallestBox);
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
            else { 
                textToCopy = textResults.Text;             
            }

            CopyText.Content = textToCopy;
            Utils.SetClipboardText(textToCopy);
            System.Diagnostics.Debug.WriteLine(textToCopy);
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                SetWindowState(AppVisualState.Minimized);
            }
        }
        private void UpdateLayoutElements()
        {
            //MainImage.Height = this.Height - 20;
            //MainImage.Width = this.Width - 20;
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayoutElements();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            //if (_isActive)
            //{
            //    //stop 
            //    _isActive = false;
            //    _timer.Stop();
            //    Execute.Content = "Start";
            //}
            //else
            //{
            //    _isActive = true;
            //    _timer.Start();
            //    Execute.Content = "Stop";

            //}

            //Utils.Activate();
            //System.Threading.Thread.Sleep(500);
            Update();

            SetWindowState(AppVisualState.WithScreenshot);

            //... and reposition
            //RECT r = Utils.GetWindowRect(Modes.WindowToWatch);
            //Utils.ChangePos(Utils.GetMainWindowsHandle(), r);
            //_timer.Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(() => this.Update()));
        }

        private async void Update()
        {
            if (Modes.WindowToWatch != IntPtr.Zero)
            {

                //Figure out what to update, and at what resolution
                double scaleFactor = (double)(int.Parse(ScaleSelection.SelectedItem.ToString())) / 100.0;


                var screen = _bitmapProvider.CaptureScreenshot();

                var imageSourceOrig = Utils.ImageSourceForBitmap(screen);
                var imageSource = RenderUtils.ScaleImage(imageSourceOrig, scaleFactor);

                //Scale

                byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);

                var results = await OcrUtils.GetOcrResults(jpegEncodedImage, scaleFactor);
                _lastOcrResults = results;

                MainImage.Source = RenderUtils.DrawOriginalBmps(imageSource, results);
            }
        }

        private void OCR_Click(object sender, RoutedEventArgs e)
        {
            //var result = OcrUtils.CatpureImage(@"C:\Users\gerhas\Desktop\tes.jpg");
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            

            //RECT r = Utils.GetWindowRect(Modes.WindowToWatch);
            
            //Utils.ChangePos(Utils.GetMainWindowsHandle(), r);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SetWindowState(AppVisualState.Minimized);

        }
    }
}
