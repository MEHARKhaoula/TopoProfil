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

namespace new_interface
{
    /// <summary>
    /// Logique d'interaction pour Zoom.xaml
    /// </summary>
    public partial class Zoom : Window
    {

        public Profil2 profil;
        public Zoom()
        {
            InitializeComponent();
        }

        private void sliZoom_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            // Make sure the control's are all ready.
            if (!IsInitialized) return;



            // Get the scale factor as a fraction 0.25 - 2.00.
            double scale = (double)(sliZoom.Value / 100.0);

            // Scale the graph.
            profil.canGraph.LayoutTransform = new ScaleTransform(scale, scale);
        }



    }
}
