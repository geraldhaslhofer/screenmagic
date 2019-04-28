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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SelectKindWindow : Window
    {
        public string SelectedItem { get; set; }

        public SelectKindWindow()
        {
            InitializeComponent();
            foreach (var item in Enum.GetValues(typeof(RegionKind)))
            {
                listBoxKinds.Items.Add(item.ToString());
            }
            buttonCancel.IsCancel = true;
            listBoxKinds.SelectionChanged += ListBoxKinds_SelectionChanged;
        }

        private void ListBoxKinds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count >0)
            {
                SelectedItem  = e.AddedItems[0].ToString();
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
