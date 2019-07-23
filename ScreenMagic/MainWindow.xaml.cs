using System;
using System.Collections.Generic;
using System.Drawing;
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
//using Windows.Graphics.Display;
//using Windows.ApplicationModel.UserActivities;
using System.Threading;

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
        private System.Windows.Forms.ContextMenu contextMenu = null;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;


        private Dictionary<string, System.Drawing.Icon> _iconHandles = null;

        private IntPtr _windowToWatch = IntPtr.Zero;
        public OcrResults _lastOcrResults = null;

        Screenshot _ux = null;
        
        IBitmapProvider _bitmapProvider;
        IOcrResultProvider _ocrProvider;

        //Watch dog for YourPhone app to appear
        System.Timers.Timer _timer = new System.Timers.Timer(2000);
        private bool _isAttachedToYourPhone = false;

        //Recording timer
        System.Timers.Timer _timerRecord = new System.Timers.Timer(1000);
        private bool _isRecording = false;

        public double _scale = 1;

        

        private void SetupUx()
        {
            ///--------------------------------------------------------------------------------------------------------------------------------------------------
            //Setup notifications icons
            ///--------------------------------------------------------------------------------------------------------------------------------------------------


            _iconHandles = new Dictionary<string, System.Drawing.Icon>();

            //Load icon from resources
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("cameramonitor.ico"));
            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                _iconHandles.Add("QuickLaunch", new System.Drawing.Icon(stream));
            }
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
            notifyIcon.Icon = _iconHandles["QuickLaunch"];
            notifyIcon.Visible = true;

            //Context menu
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();

            // Initialize menuItem1
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "E&xit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            this.menuItem2.Index = 1;
            this.menuItem2.Text = "&Start recording";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);

            this.menuItem3.Index = 2;
            this.menuItem3.Text = "S&top recording";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);

            this.menuItem4.Index = 3;
            this.menuItem4.Text = "&Experimental";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);


            // Initialize contextMenus
            this.contextMenu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.menuItem1, this.menuItem2, this.menuItem3, this.menuItem4 });


            this.notifyIcon.ContextMenu = contextMenu;
        }

        private void SetRecordingUx()
        {
            if (_isRecording)
            {
                menuItem2.Enabled = false; //Recording is now disabled
                menuItem3.Enabled = true;
            }
            else
            {
                menuItem2.Enabled = true; //Recording is now disabled
                menuItem3.Enabled = false;
            }
        }


        public MainWindow()
        {
            _isRecording = false;

            SetupUx();
            SetRecordingUx();
            _timerRecord.Elapsed += _timerRecord_Elapsed;
            Recording.EnsureWorkingDirectory();
            
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

        private async void _timerRecord_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            _timerRecord.Enabled = false;
            Bitmap bmp;
            CaptureContext ctx;
            _bitmapProvider.CaptureScreenshot(out bmp, out ctx);
            await Recording.ProcessAndPersistScreenshot(bmp, false);

            if (_isRecording)
            {
                _timerRecord.Enabled = true;
            }
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

        private async void notifyIcon_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException("Todo");
            ////Persist image to disk
            //var screen = _bitmapProvider.CaptureScreenshot();
            //var res = await Recording.ProcessAndPersistScreenshot(screen);
            //string text = res.Results.GetRawText();

           
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Execute_Click(null, null);    
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            // Close the form, which closes the application.
            System.Windows.Application.Current.Shutdown();
        }

        //Start recording
        private void menuItem2_Click(object Sender, EventArgs e)
        {
            _isRecording = true;
            SetRecordingUx();
            _timerRecord.Enabled = true;
        }

        //Stop recording
        private void menuItem3_Click(object Sender, EventArgs e)
        {
            _isRecording = false;
            _timer.Enabled = false;
            SetRecordingUx();
        }
        //Stop recording
        private async void menuItem4_Click(object Sender, EventArgs e)
        {
            
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
   
        private  async void Execute_Click(object sender, RoutedEventArgs e)
        {
            Update();
            SetWindowState(AppVisualState.WithScreenshot);
            
        }
        
        private async void UpdateCached()
        {
            throw new NotImplementedException("Todo");

            //var screen = _bitmapProvider.CaptureScreenshot();

            //var imageSourceOrig = Utils.ImageSourceForBitmap(screen);
            //var imageSource = RenderUtils.ScaleImage(imageSourceOrig, 1);
            
            ////Render before we try to do OCR 

            //_ux.SetImage(RenderUtils.DrawOriginalBmps(imageSource, null), _bitmapProvider.ScaleFactor());

            ////Scale

            //byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);

            //var results = await OcrUtils.GetOcrResults(jpegEncodedImage, 1);
            //_lastOcrResults = results;
            //_ux.SetImage(RenderUtils.DrawOriginalBmps(imageSource, results), _bitmapProvider.ScaleFactor());
            
        }

        private async void Update()
        {
            if (Config.WindowToWatch != IntPtr.Zero)
            {
                CaptureContext ctx;
                Bitmap screen;

                _bitmapProvider.CaptureScreenshot(out screen, out ctx);

                var imageSourceOrig = Utils.ImageSourceForBitmap(screen);
                //var imageSource = RenderUtils.ScaleImage(imageSourceOrig, scaleFactor);


                //Render before we try to do OCR 
                
                _ux.SetImage(RenderUtils.DrawOriginalBmps(imageSourceOrig, null), ctx);

                //Scale

                byte[] jpegEncodedImage = Utils.SerializeBitmapToJpeg(screen);

                var results = await OcrUtils.GetOcrResults(jpegEncodedImage, 1);
                _lastOcrResults = results;
                
                _ux.SetImage(RenderUtils.DrawOriginalBmps(imageSourceOrig, results), ctx);
            }
        }

      
    }
}
