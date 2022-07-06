
    namespace new_interface
{  /// <summary>
   /// Logique d'interaction pour pte.xaml
   /// </summary>
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

    /// <summary>
    /// Logique d'interaction pour pte.xaml
    /// </summary>
    public partial class pte : Window
        {
            public pte(Double pi)
            {
                InitializeComponent();
                p.Content = Convert.ToString(pi);
            }
            private void btnDialogOk_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }
            private void Window_ContentRendered(object sender, EventArgs e)
            {







            }


            private void ButtonClose_Click(object sender, RoutedEventArgs e)
            {
            this.Close();
            }
        }
    }


