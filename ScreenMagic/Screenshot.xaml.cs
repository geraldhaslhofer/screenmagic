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
using System.Diagnostics;

namespace ScreenMagic
{
    /// <summary>
    /// Interaction logic for Screenshot.xaml
    /// </summary>
    public partial class Screenshot : Window
    {
        private static string SERIALIZE_PATH = @"c:\test\";
        //For Tagging mode ---------------------------------------
        Tagger _tagger = new Tagger(SERIALIZE_PATH);

        // -------------------------------------------------------
        MainWindow _mainWindow = null;

        // Interaction states
        bool _isMouseDrag = false;
        System.Drawing.Point _startSelection = new System.Drawing.Point();
        System.Drawing.Point _endSelection = new System.Drawing.Point();

        BitmapSource _originalBitmap = null;
        CaptureContext _ctx = null;

        //Animation timer to have Window disappear after text has been copied
        System.Timers.Timer _timer = new System.Timers.Timer(2000);

        public Screenshot(MainWindow ownerWindow)
        {
            InitializeComponent();
            _mainWindow = ownerWindow;
            

            StatusMessage.Visibility = Visibility.Hidden;
            StatusMessageInternal.Visibility = Visibility.Hidden;

            MainImage.MouseDown += MainImage_MouseDown;

            //Set up selection of Text
            MainImage.MouseLeftButtonDown += MainImage_MouseLeftButtonDown;
            MainImage.MouseLeftButtonUp += MainImage_MouseLeftButtonUp;
            MainImage.MouseMove += MainImage_MouseMove;

            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(() => this.HideWindow()));

        }

        //-----------------------------------------------------------------------------------------------
        // Mouse movement during selection
        //-----------------------------------------------------------------------------------------------

        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Start selection
            _isMouseDrag = true;

            //Remember starting position
            _startSelection = System.Windows.Forms.Cursor.Position;
            Debug.WriteLine("LeftMouseDown");
        }

        private System.Drawing.Rectangle GetSelectionPhysicalRelative()
        {
            System.Drawing.Rectangle selection = new System.Drawing.Rectangle();
            selection.X = Math.Min(_startSelection.X, _endSelection.X);
            selection.Y = Math.Min(_startSelection.Y, _startSelection.Y);
            selection.Width = Math.Abs(_endSelection.X - _startSelection.X);
            selection.Height= Math.Abs(_endSelection.Y - _startSelection.Y);

            System.Drawing.Rectangle relativeLogicalScreen = GlobalUtils.MonitorHelper.GetRelativeRectangle(_ctx.CapturedWindowLogical, selection);
            
            //Now scale to physical 
            System.Drawing.Rectangle relativePhysical = GlobalUtils.MonitorHelper.ScaleRect(relativeLogicalScreen, _ctx.ScaleFactor);
            return relativePhysical;

        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDrag)
            {
                _endSelection = System.Windows.Forms.Cursor.Position;
                //Create bitmap with the selected rectangle

                //Translate the absolute logical coordinate into the relative physical coordinate on the screen

                System.Drawing.Rectangle selection = GetSelectionPhysicalRelative();

                var imageWithRect = GlobalUtils.BitmapManipulation.DrawSelectionRectangle(_originalBitmap, selection);

                if (Config.IsTaggerMode())
                {
                    List<GlobalUtils.DrawingElement> elems = new List<GlobalUtils.DrawingElement>();
                    foreach (var item in _tagger.GetTaggedRegions())
                    {
                        GlobalUtils.DrawingElement elem = new GlobalUtils.DrawingElement();
                        elem.rect = item.RegionRect;
                        elem.brush = Brushes.Green;
                        elem.pen = new Pen(Brushes.Green, 1.0);
                        elems.Add(elem);
                            
                    }

                    //Tag selected regions
                    imageWithRect = GlobalUtils.BitmapManipulation.DrawBoxes(imageWithRect, elems);

                }

                //var imageWithRectFinal = RenderUtils.(_originalBitmap, _startSelection, _endSelection);
                MainImage.Source = imageWithRect;
                Debug.WriteLine("MouseMove");
            }
        }

        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseDrag)
            {
                var p = System.Windows.Forms.Cursor.Position;
                //Completed a selection, now copy to clipboard
                _endSelection = System.Windows.Forms.Cursor.Position;
                _isMouseDrag = false;
                Debug.WriteLine("Completed selection from: " + _startSelection.ToString() + " end: " + _endSelection.ToString());

                //System.Drawing.Rectangle selectionLogical = new System.Drawing.Rectangle((int)Math.Min(_startSelection.X, _endSelection.X),
                //                                                                         (int)Math.Min(_startSelection.Y, _endSelection.Y),
                //                                                                         (int)Math.Abs(_endSelection.X - _startSelection.X),
                //                                                                         (int)Math.Abs(_endSelection.Y - _startSelection.Y));


                //Scale to physical dimension of image

                

                System.Drawing.Rectangle selectionPhysical = GetSelectionPhysicalRelative();

                //In tagging mode add rectangle and pop question of area semantics
                if (Config.IsTaggerMode())
                {
                    SelectKindWindow w = new SelectKindWindow();
                    bool? result = w.ShowDialog();
                    if (result != null && result == true)
                    {
                        RegionKind k = (RegionKind)Enum.Parse(typeof(RegionKind), w.SelectedItem);
                        Debug.WriteLine("OK clicked");

                        Region r = new Region();
                        r.Kind = k;
                        r.RegionRect = selectionPhysical;
                        //Add to the tagged region
                        _tagger.AddRegion(r);
                        Debug.WriteLine(r.SerializeToCsv());
                    }
                }
                else
                {


                    string copiedText = GetTextFromScreenRect(selectionPhysical);
                    MainImage.Source = null;

                    StatusMessageInternal.Text = "Copied text to clipboard" +
                        Environment.NewLine +
                        Environment.NewLine +
                        copiedText;

                    Utils.SetClipboardText(copiedText);

                    StatusMessage.Visibility = Visibility.Visible;
                    StatusMessageInternal.Visibility = Visibility.Visible;

                    Debug.WriteLine("Detected text:" + copiedText);

                    //Start timer to remove window
                    _timer.Enabled = true;

                    //Render original image
                }
            }

        }


        private void HideWindow()
        {
            MainImage.Source = null;
            StatusMessageInternal.Text= String.Empty;
            this.Hide();
        }

       
        

        public void SetImage(BitmapSource img, CaptureContext ctx)
        {
            _ctx = ctx;
            _originalBitmap = img;
            _tagger.SetBitmap(img);
            MainImage.Source = _originalBitmap;
        }

        private string GetTextFromScreenRect(System.Drawing.Rectangle r)
        {
            //Point p1_translated = TranslateScreenPosToImagePos(p1);
            //Point p2_translated = TranslateScreenPosToImagePos(p2);
            BoundingBox b = new BoundingBox();
            b.X = r.X;
            b.Y = r.Y;
            b.Width = r.Width;
            b.Height = r.Height;
            
            var allOcrResults = _mainWindow._lastOcrResults.GetOcrResultFromBoundingbox(b);
            if (allOcrResults != null && allOcrResults.Count > 0)
            {
                StringBuilder res = new StringBuilder();
                foreach (var aRes in allOcrResults)
                {
                    res.Append(aRes.Text);
                    res.Append(" ");
                }
                return res.ToString();
            }

            return String.Empty;

        }
        private Point TranslateScreenPosToImagePos(Point p)
        {
            ImageSource imageSource = MainImage.Source;
            RenderTargetBitmap bitmapImage = (RenderTargetBitmap)imageSource;
            var pixelMousePositionX = p.X * bitmapImage.PixelWidth / MainImage.Width;
            var pixelMousePositionY = p.Y * bitmapImage.PixelHeight / MainImage.Height;


            double finalClickPosX = p.X * _mainWindow._scale;
            double finalClickPosY = p.Y * _mainWindow._scale;

            return new Point(finalClickPosX, finalClickPosY);

        }
        private void MainImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var clickPos = e.GetPosition(MainImage);

            ////Figure out position
            //ImageSource imageSource = MainImage.Source;
            //RenderTargetBitmap bitmapImage = (RenderTargetBitmap)imageSource;
            //var pixelMousePositionX = e.GetPosition(MainImage).X * bitmapImage.PixelWidth / MainImage.Width;
            //var pixelMousePositionY = e.GetPosition(MainImage).Y * bitmapImage.PixelHeight / MainImage.Height;

            //System.Diagnostics.Debug.WriteLine(clickPos.ToString() + " " + pixelMousePositionX.ToString() + " " + pixelMousePositionY.ToString());

            //double finalClickPosX = clickPos.X * _mainWindow._scale;
            //double finalClickPosY = clickPos.Y * _mainWindow._scale;

            //var textResults = _mainWindow._lastOcrResults.GetOcrResultFromCoordinate((int)finalClickPosX, (int)finalClickPosY);

            //string textToCopy = string.Empty;

            //if (textResults == null)
            //{
            //    // try outer bounding box

            //    var smallestBox = _mainWindow._lastOcrResults.GetSmallestBoundingBox((int)clickPos.X, (int)clickPos.Y);
            //    if (smallestBox != null)
            //    {
            //        var allOcrResults = _mainWindow._lastOcrResults.GetOcrResultFromBoundingbox(smallestBox);
            //        if (allOcrResults != null && allOcrResults.Count > 0)
            //        {
            //            StringBuilder b = new StringBuilder();
            //            foreach (var aRes in allOcrResults)
            //            {
            //                b.Append(aRes.Text);
            //                b.Append(" ");
            //            }
            //            textToCopy = b.ToString();
            //        }  
            //    } 
             
            //} 
            //else
            //{ 
            //    textToCopy = textResults.Text;
            //}

            //_mainWindow.SetCopyText(textToCopy);
            //Utils.SetClipboardText(textToCopy);
            //System.Diagnostics.Debug.WriteLine(textToCopy);
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            this.HideWindow();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //persist
            _tagger.Persist();
            _tagger = new Tagger(SERIALIZE_PATH);
            this.HideWindow();
        }
    }
}
