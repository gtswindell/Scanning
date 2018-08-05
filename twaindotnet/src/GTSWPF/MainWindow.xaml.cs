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

namespace GTSWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            twainPreview.SelectSource();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            twainPreview.UseADF = false;
            twainPreview.UseColour = true;
            twainPreview.UseDuplex = true;
            twainPreview.ShowGUI = twainPreview.ShowProgress = true;
            twainPreview.Scan();
        }
    }
}
