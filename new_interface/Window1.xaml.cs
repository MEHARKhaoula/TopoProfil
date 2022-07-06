using System;

using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Reflection;



namespace new_interface
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public Profil2 w1; // la fenetre profil qui subit les modifications 
        public Window1()
        {
            InitializeComponent();
            c3.ItemsSource = typeof(Colors).GetProperties();
            c4.ItemsSource = typeof(Colors).GetProperties();
        }
        private void Modify_click(object sender, RoutedEventArgs e)
        {
            if (c1.SelectedItem != null)
            {
                String x = (String)c1.Text; // la largeur choisie dans le combox
                int ch1 = Convert.ToInt32(x);
                String y = (String)c2.Text; // la longueur choisie dans le combox
                int ch2 = Convert.ToInt32(y);
                this.w1.canGraph.Height = ch1 + 40;//sera la nouvelle longueur de canvas canGraph
                this.w1.dxmax = ch2 + 40; // sera la nouvelle largeur du canvas canGraph
                int index = c3.SelectedIndex;

                this.w1.canGraph.Children.Clear();
                // on fait à la méthode  de la classe profile
                w1.Modifier_Loaded();  
                


            }
            this.Close();


        }

        private void cmbColors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)

        {
            // la nouvelle couleur du graphe
            w1.color = (Color)(c3.SelectedItem as PropertyInfo).GetValue(null, null);

        }

        private void cmbColors_SelectionChanged2(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // la nouvelle background du graphe 
            w1.back = (Color)(c4.SelectedItem as PropertyInfo).GetValue(null, null);

        }


        private void c1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
