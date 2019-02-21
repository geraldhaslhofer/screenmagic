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
        WithScreenshot = 1,
        Minimized = 3
    }



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Notifications area
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private Dictionary<string, System.Drawing.Icon> _iconHandles = null;

        private IntPtr _windowToWatch = IntPtr.Zero;
        public OcrResults _lastOcrResults = null;

        Screenshot _ux = null;
        
        IBitmapProvider _bitmapProvider;
        IOcrResultProvider _ocrProvider;

        //Watch dog for YourPhone app to appear
        System.Timers.Timer _timer = new System.Timers.Timer(2000);
        private bool _isAttachedToYourPhone = false;


        public double _scale = 1;
        public MainWindow()
        {

            ///--------------------------------------------------------------------------------------------------------------------------------------------------
            //Setup notifications icons

            _iconHandles = new Dictionary<string, System.Drawing.Icon>();

            _iconHandles.Add("QuickLaunch", new System.Drawing.Icon(System.IO.Path.Combine(Environment.CurrentDirectory, @"Assets\cameramonitor.ico")));

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
            notifyIcon.Icon = _iconHandles["QuickLaunch"];
            notifyIcon.Visible = true;

            ///--------------------------------------------------------------------------------------------------------------------------------------------------

            _scale = Utils.GetScale(this);
            InitializeComponent();
            _bitmapProvider = BitmapProviderFactory.GetBitmapProvider();
            _ocrProvider = OcrProviderFactory.GetOcrResultsProvider();

            PopulateListOfApps();
            AppSelection.SelectionChanged += AppSelection_SelectionChanged;
            Execute.IsEnabled = false;

            _ux = new Screenshot(this);
            
            
            SetWindowState(AppVisualState.Minimized);

            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            var yourPhone = ProcessHelpers.GetYourPhoneWindow();
            if (yourPhone != null)
            {
                // The YourPhone app is available
                if (_isAttachedToYourPhone == false)
                {
                    // new attach
                    _isAttachedToYourPhone = true;
                    Config.WindowToWatch = yourPhone.Handle;

                    //Hide UX (only Notify icon area remains)

                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(() => this.Hide()));
                    
                }
            }

            _timer.Enabled = true ;
            
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {

            //ShowSysTrayMenu();

        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Execute_Click(null, null);
            
        }



        public void SetCopyText(string text)
        {
            CopyText.Content = text;
        }

        private void SetWindowState(AppVisualState state)
        {
            switch (state) {
                case AppVisualState.Minimized:
                    {
                        
                        _ux.WindowState= WindowState.Minimized;
                        _ux.Hide();
                    }
                    break;
                case AppVisualState.WithScreenshot:
                    {
                        _ux.Show();
                        _ux.WindowState = WindowState.Normal;
                        //... move over app to watch
                        RECT r = Utils.GetWindowRect(Config.WindowToWatch);
                   
                        _ux.Top = r.Top / _scale;
                        _ux.Left = r.Left / _scale;
                        _ux.Width = (r.Right - r.Left)/_scale;
                        _ux.Height = (r.Bottom - r.Top)/_scale;
                        

                    } break;
                
            }
        }

      
        
        private void AppSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DesktopWindow w = (DesktopWindow)AppSelection.SelectedItem;
            Config.WindowToWatch = w.Handle;
            Execute.IsEnabled = true;
        }

        private void PopulateListOfApps()
        {
            AppSelection.Items.Clear();
            
            var visible = ProcessHelpers.GetActiveWindows();
            foreach (var avisible in visible)
            {
                AppSelection.Items.Add(avisible);
            }

        }
        //private void MainImage_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var clickPos = e.GetPosition(MainImage);

        //    //Figure out position
        //    ImageSource imageSource = MainImage.Source;
        //    RenderTargetBitmap bitmapImage = (RenderTargetBitmap)imageSource;
        //    var pixelMousePositionX = e.GetPosition(MainImage).X * bitmapImage.PixelWidth / MainImage.Width;
        //    var pixelMousePositionY = e.GetPosition(MainImage).Y * bitmapImage.PixelHeight / MainImage.Height;

        //    System.Diagnostics.Debug.WriteLine(clickPos.ToString() + " " + pixelMousePositionX.ToString() + " "+ pixelMousePositionY.ToString());

            
        //    var textResults = _lastOcrResults.GetOcrResultFromCoordinate((int)clickPos.X, (int)clickPos.Y);

        //    string textToCopy = string.Empty;

        //    if (textResults == null)
        //    {
        //        // try outer bounding box

        //        var smallestBox  = _lastOcrResults.GetSmallestBoundingBox((int)clickPos.X, (int)clickPos.Y);
        //        if (smallestBox != null)
        //        {
        //            var allOcrResults = _lastOcrResults.GetOcrResultFromBoundingbox(smallestBox);
        //            if (allOcrResults != null && allOcrResults.Count > 0)
        //            {
        //                StringBuilder b = new StringBuilder();
        //                foreach (var aRes in allOcrResults)
        //                {
        //                    b.Append(aRes.Text);
        //                    b.Append(" ");
        //                }
        //                textToCopy = b.ToString();
        //            }
        //        }

        //    }
        //    else { 
        //        textToCopy = textResults.Text;             
        //    }

        //    CopyText.Content = textToCopy;
        //    Utils.SetClipboardText(textToCopy);
        //    System.Diagnostics.Debug.WriteLine(textToCopy);
        //}

        //private void MainWindow_StateChanged(object sender, EventArgs e)
        //{
        //    if (this.WindowState == WindowState.Minimized)
        //    {
        //        SetWindowState(AppVisualState.Minimized);
        //    }
        //    else if (this.WindowState == WindowState.Normal)
        //    {
        //        Update();
        //        SetWindowState(AppVisualState.WithScreenshot);
        //    }
        //}
        //private void UpdateLayoutElements()
        //{
        //    //MainImage.Height = this.Height - 20;
        //    //MainImage.Width = this.Width - 20;
        //}
        //private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    UpdateLayoutElements();
        //}

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

        //private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
        //         new Action(() => this.Update()));
        //}

        private async void Update()
        {
            if (Config.WindowToWatch != IntPtr.Zero)
            {

                //Figure out what to update, and at what resolution
                double scaleFactor = 1;/// _scale;


                var screen = _bitmapProvider.CaptureScreenshot();

                var imageSourceOrig = Utils.ImageSourceForBitmap(screen);
                var imageSource = RenderUtils.ScaleImage(imageSourceOrig, scaleFactor);


                //Render before we try to do OCR 
                
                _ux.SetImage(RenderUtils.DrawOriginalBmps(imageSource, null));

                //Scale

                byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);

                var results = await OcrUtils.GetOcrResults(jpegEncodedImage, scaleFactor);
                _lastOcrResults = results;

                
                _ux.SetImage(RenderUtils.DrawOriginalBmps(imageSource, results));
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
