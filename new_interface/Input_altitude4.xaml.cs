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
    /// Logique d'interaction pour Input_altitude3.xaml
    /// </summary>
    public partial class Input_altitude4 : Window
    {
        public Input_altitude4(string question, string defaultAnswer = "")
        {
            InitializeComponent();

            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {



            txtAnswer.SelectAll();
            txtAnswer.Focus();



        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
