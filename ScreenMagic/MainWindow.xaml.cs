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
        public MainWindow()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            
            _windowToWatch = Utils.Activate();
            if (_windowToWatch != IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(2000);
                _timer.Start();

            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(() => this.Update()));

        }

        private void Update( )
        {
            var screen = Utils.CaptureScreenshot(_windowToWatch);
            MainImage.Source = Utils.ImageSourceForBitmap(screen);

            //Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            //        new Action(() => this.Update()));

        }
    }
}
