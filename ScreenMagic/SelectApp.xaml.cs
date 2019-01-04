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
    /// Interaction logic for SelectApp.xaml
    /// </summary>
    public partial class SelectApp : Window
    {
        public SelectApp()
        {
            InitializeComponent();

            var windows = User32Helper.GetDesktopWindows();
            var visible = from x in windows where x.IsVisible && x.Title.Length > 2 orderby x.Title select x ;
            foreach (var avisible in visible)
            {
                AppsList.Items.Add(avisible);
            }
            
        }

        private void AppsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DesktopWindow w = (DesktopWindow)AppsList.SelectedItem;
            Modes.WindowToWatch = w.Handle;
            this.Close();
        }
    }
}
