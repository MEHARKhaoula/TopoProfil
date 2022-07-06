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

namespace new_interface
{
    /// <summary>
    /// Logique d'interaction pour aide.xaml
    /// </summary>
    public partial class aide : Page
    {
        public aide()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("file:///C:/aide1.html");


        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var back = new MainWindow();
            back.Show();
            back.DataContext = new MainWindow();
        }

    }
}
