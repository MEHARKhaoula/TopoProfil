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
    /// Logique d'interaction pour Input_info.xaml
    /// </summary>
    public partial class Input_info1 : Window
    {
        public Input_info1(string question1, string question2, string question3, string question4, string question5, string question, string defaultAnswer1 = "", string defaultAnswer2 = "", string defaultAnswer3 = "", string defaultAnswer4 = "", string defaultAnswer5 = "", string defaultAnswer = "")
        {
            InitializeComponent();
            lblQuestion1.Content = question1;
            txtAnswer1.Text = defaultAnswer1;
            lblQuestion2.Content = question2;
            txtAnswer2.Text = defaultAnswer2;
            lblQuestion3.Content = question3;
            txtAnswer3.Text = defaultAnswer3;
            lblQuestion4.Content = question4;
            txtAnswer4.Text = defaultAnswer4;
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
            lblQuestion5.Content = question5;
            txtAnswer5.Text = defaultAnswer5;

        }
        // c'est le bouton qui permet de sauvegarder les resultats dans des listes
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        // dans cette méthode on utilise des méthodes focus pour verifier si les controle
        //a bien reçu les informations entrées
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer1.SelectAll();
            txtAnswer1.Focus();
            txtAnswer2.SelectAll();
            txtAnswer2.Focus();
            txtAnswer3.SelectAll();
            txtAnswer3.Focus();
            txtAnswer4.SelectAll();
            txtAnswer4.Focus();

            txtAnswer.SelectAll();
            txtAnswer.Focus();

            txtAnswer5.SelectAll();
            txtAnswer5.Focus();


        }
        public string Answer1//pour récupérer le min
        {
            get { return txtAnswer1.Text; }

        }
        public string Answer2//pour récupérer le max
        {
            get { return txtAnswer2.Text; }

        }
        public string Answer3 //pour récupérer l'equidistance
        {
            get { return txtAnswer3.Text; }

        }
        public string Answer4//pour récupérer l'echelle
        {
            get { return txtAnswer4.Text; }

        }
        public string Answer//pour récupérer l'unité de l'altitude
        {
            get { return txtAnswer.Text; }
        }
        public string Answer5//pour récupérer l'unité de distance
        {
            get { return txtAnswer5.Text; }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)//pour fermer la page
        {
            this.Close();
        }
        //pour déplacer la page
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
