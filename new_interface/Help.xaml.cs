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
    /// Logique d'interaction pour Help.xaml
    /// </summary>
    /// /// pour afficher la page d'aide ne ligne d'application
    public partial class Help : Window
    {

        public Help()
        {
            InitializeComponent();
            
           
        }
        // cette méthode êrmet d'ouvrir la^page html à partir d'un navigateur
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = @"aide\help.html";// c'est le schema relative de fichier
                System.Diagnostics.Process.Start(path);//l'accés au fichier
                this.Close();
            }
            catch
            {
                MessageBox.Show("Vous ne pouvez pas ouvrir l'aide en ligne vérifier que tu as installé le dossier aide dans disque local 'c' ");
            }

        }

        
    }
}
