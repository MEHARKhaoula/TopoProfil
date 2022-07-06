using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using Ab2d.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using static System.IO.File;
using static System.IO.Stream;
using static System.IO.FileMode;
using static System.IO.StreamReader;
using static System.IO.StreamWriter;
using static System.IO.TextReader;
using static System.IO.TextWriter;


namespace new_interface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class
        MainWindow2 : Window
    {
        bool supp2 = false;
        public static MainWindow2 page;
        int selected = 0;
        private List<int> maximum = new List<int>();//liste pour stocker le max de la carte
        private List<int> minimum = new List<int>();//liste pour stocker le min de la carte
        private List<int> equidis = new List<int>();//liste pour stocker l'equidistance de la carte
        private List<double> echell = new List<double>();//pour stocker l'échelle
        private List<string> Unite = new List<string>();//liste pour stocker l'unité de distance du graphe
        private List<string> Uniteal = new List<string>();//liste pour stocker l'unité des altitudes des courbes
        List<Tuple<List<Point>, List<int>>> allData = new List<Tuple<List<Point>, List<int>>>();
        List<Tuple<int, int, int, double, String, String>> allpente = new List<Tuple<int, int, int, double, String, String>>();//liste pour serialiser les données 
        private int max, min, equidistance, alitude2;
        private double echelle;
        private string united, uniteal;
        private List<int> commands = new List<int>();
        private int lastExecuted = -1;
        private int lastSaved = -1;
        public delegate void Changed(bool haveUnsavedChanges);
        public event Changed OnChanged = (h) => { };
        Curve curv = new Curve();
        Curve SegmentAB = new Curve();
        int draw = 0;
        int color = 2;
        private List<Curve> curvelist = new List<Curve>();
        bool stop = false;
        int cp = 0;
        private List<Ellipse> circle_list = new List<Ellipse>();
        private FrameworkElement selectedline;
        private Curve selectedcurve;
        private Boolean cut;
        private Boolean pointmoved; //indiquer que le point va etre deplace
        private int indexselected;
        private int indexpointselected = 0;
        Curve copiedcurve;
        private Ellipse selectedpoint = null;
        bool trouv = false;
        Point pointcourant;//se reperer en faisant bouger le point
        Segment seg;
        Dictionary<Point, int> intersection_points = new Dictionary<Point, int>();
        private List<int> altitudes = new List<int>();//liste pour stocker les altitudes
        private List<Point> coord = new List<Point>();
        private List<double> distances = new List<double>();
        List<Ellipse> intersection_ellipses = new List<Ellipse>();
        private bool trouvAB = true;

        bool ajouter; bool mouve = false;
        Segment seg_mouve = new Segment();
        Segment L2 = new Segment();

        private double dxmax;
        private int altitude2;
        private bool suppr, mouved;

        private struct PixelUnitFactor
        {
            public const double Px = 1.0;
            public const double Inch = 96.0;
            public const double Cm = 37.7952755905512;
            public const double Pt = 1.33333333333333;
        }


        public double CmToPx(double cm)
        {
            return cm * PixelUnitFactor.Cm;
        }

        public double PxToCm(double px)
        {
            return px / PixelUnitFactor.Cm;
        }
        public MainWindow2()
        {
            System.Threading.Thread.Sleep(1000);//pour afficher le logo lors de l'ouverture de l'applicatio
            InitializeComponent();
            /*Image imageviewer = new Image();

            scrollViewer.Content = imageviewer;
            imageviewer = ImgOriginal;
            ScaleTransform scaleTransform = new ScaleTransform();*/

            //imageviewer.LayoutTransform = scaleTransform;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        // des getters et des setters pour les variables stockées dans les listes
        public int MAX { get => max; set => max = value; }
        public int MIN { get => min; set => min = value; }
        public int Equidistance { get => equidistance; set => equidistance = value; }
        public double Echelle { get => echelle; set => echelle = value; }



        //pour fermer l'application
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }
        //pour ouvrir le menu de l'application
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }
        /* private void ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
         {
             if (ImgOriginal != null)
             {
                 double scale = (double)(Zoom.Value / 100.0);
                 ImgOriginal.LayoutTransform = new ScaleTransform(scale, scale);

             }

         }*/


        //pour effectuer des déplacements sur l'image
        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                Canvas.SetTop(ImgOriginal, Canvas.GetTop(ImgOriginal) + 10);
            }
            else if (e.Key == Key.Up)
            {
                Canvas.SetTop(ImgOriginal, Canvas.GetTop(ImgOriginal) - 10);
            }
            else if (e.Key == Key.Left)
            {
                Canvas.SetLeft(ImgOriginal, Canvas.GetLeft(ImgOriginal) - 10);
            }
            else if (e.Key == Key.Right)
            {
                Canvas.SetLeft(ImgOriginal, Canvas.GetLeft(ImgOriginal) + 10);
            }
        }
        //pour obtenir la position des points dans le grid principale
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            //var point = e.GetPosition(this.YourControl);
            var relativePosition = e.GetPosition(this);
            var point = PointToScreen(relativePosition);
            // _x.HorizontalOffset = point.X;
            // _x.VerticalOffset = point.Y;
        }


        /* private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
         {
             var image = e.Source as Image;

             if (ImgOriginal != null && canvas.CaptureMouse())
             {
                 mousePosition = e.GetPosition(canvas);
                 draggedImage = image;
                 Panel.SetZIndex(ImgOriginal, 1); // in case of multiple images
             }
         }*/

        /*  private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
          {
              if (ImgOriginal != null)
              {
                  canvas.ReleaseMouseCapture();
                  Panel.SetZIndex(ImgOriginal, 0);
                  ImgOriginal = null;
              }
          }*/

        /*  private void CanvasMouseMove(object sender, MouseEventArgs e)
          {
              if (ImgOriginal != null)
              {
                  var position = e.GetPosition(canvas);
                  var offset = position - mousePosition;
                  mousePosition = position;
                  Canvas.SetLeft(ImgOriginal, Canvas.GetLeft(ImgOriginal) + offset.X);
                  Canvas.SetTop(ImgOriginal, Canvas.GetTop(ImgOriginal) + offset.Y);
              }
          }*/

        private void undo()
        {
            if (lastExecuted >= 0)
            {
                if (commands.Count > 0)
                {
                    //commands[lastExecuted].Undo();
                    lastExecuted--;
                    OnChanged(lastExecuted != lastSaved);
                }
            }
        }
        // c'est le bouton ouvrir il permet d'ouvrir le travail sauvegardé 
        private void redo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();// variable pour ouvrir la fentre des fichier
            if (open.ShowDialog() == true)
            {
                var src = new Uri(open.FileName);
                LoadThings(src.ToString().Substring(8));//appel à la méthode d'ouverture des données
            }
        }
        // ce module permet d'animer le menu
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }
        //ce bouton permet de sauvegarder le travail plusieurs fois
        private void ButtonSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog open = new SaveFileDialog();//ouverture de la fenetre de sauvegarde
            open.DefaultExt = "txt";
            open.Filter = "text files(*.txt) | *.txt | All files (*.*)|*.*";//l'extension de sauvegarde
            if (open.ShowDialog() == true)
            {
                var src = new Uri(open.FileName);
                SaveThings(src.ToString().Substring(8));//appel aux méthodes de sauvegarde des fichiers
            }

        }
        //méthode de sauvegarde d'image
        void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }
        void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        // pour accéder a la pge d'acceuil
        private void listViewItem_Selected(object sender, RoutedEventArgs e)
        {

            var y = new MainWindow();// on navigue à une autre fenetre principale
            y.Show();
            this.Close();
            /*
             * if (this.WindowState == WindowState.Normal)
            {
                GridMain.Width = 1365;

                //ImgOriginal.Height = 800;
                this.WindowState = WindowState.Maximized;
                GridMain.Height = 900;
                src.Width = 1024;
                src.Height = 820;
                canvas.Width = 1020;
                canvas.Height = 800;

                ImgOriginal.Width = 1015;
                sliZoom.Value = 130;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                // this.Width = SystemParameters.WorkArea.Width;
                //this.Height = SystemParameters.WorkArea.Height;
                this.WindowState = WindowState.Normal;
                GridMain.Width = 1024;
                GridMain.Height = 600;
                src.Width = 713;
                canvas.Width = 700;
                ImgOriginal.Width = 690;
                sliZoom.Value = 100;
            }*/

        }
        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;

        }
        Input_info1 y;
        // cette méthode pour importer une image et entrer des informations nécessaires
        private void listViewItem1_Selected(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();//la fentre d'ouverture
            // indiquer les extensions des images qu'on puisse les ouvrir
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";
            if (dialog.ShowDialog() == true)
            {

                var src = new Uri(dialog.FileName);
                ImgOriginal.Source = new BitmapImage(src);
                bool except = false;

                //traiter les exceptions
                while (except == false)
                {
                    try
                    {
                        //ouvrir la fentre de remplissage
                        y = new Input_info1("MIN", "MAX", "equid", "Echelle", "Unite", "Unitealt ", " ", " ", " ", " ", " ", " ");

                        if (y.ShowDialog() == true)
                        {
                            //lire les informations entrées
                            MIN = Int32.Parse(y.Answer1);
                            MAX = Int32.Parse(y.Answer2);
                            Equidistance = Int32.Parse(y.Answer3);
                            Echelle = Convert.ToDouble(y.Answer4);
                            united = y.Answer5;
                            uniteal = y.Answer;
                        }
                        //le min et le max et l'altitude doit etre multiple dec5
                        if ((MAX % 5 != 0) || (MIN % 5 != 0) || (Equidistance % 5 != 0))
                        {
                            MessageBox.Show("La valeur de l'équidistance et le min et le max doivent etre des multiples de 5");
                            y.Close();
                        }

                        else
                        {

                           
                            
                            
                                if ((MIN > MAX))
                                {
                                    MessageBox.Show("La valeur du min doit etre inférieur au max");
                                    y.Close();
                                }
                                else
                                {
                                    // ajouter les informations entrées dans des listes pour les récupérer après
                                    MessageBox.Show("enregistré avec succés");
                                    maximum.Add(MAX);
                                    minimum.Add(MIN);
                                    equidis.Add(Equidistance);
                                    echell.Add(Echelle);

                                    Unite.Add(united);
                                    Uniteal.Add(uniteal);
                                    except = true;
                                    allpente.Add(new Tuple<int, int, int, double, String, String>(MAX, MIN, Equidistance, Echelle, united, uniteal));
                                }
                            }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veuillez respecter le format des données");
                        y.Close();
                    }
                }

            }
            else
            { //MessageBox.Show("Vous devez d'abord importez une image");
            }

        }




        private void listViewItem2_Selected(object sender, RoutedEventArgs e)
        {
            Curve curv1 = new Curve();
            curv1.inc_numordre();
            curv = curv1;
            draw = 1;


        }



        private void listViewItem3_Selected(object sender, RoutedEventArgs e)
        {

            if ((SegmentAB.SegmentList.Count == 0))
            {
                foreach (Ellipse ell in circle_list)
                {
                    canvas.Children.Remove(ell);
                }
                circle_list.Clear();
                draw = 2; //on va dessiner le segment AB
                SegmentAB = new Curve();
                SegmentAB.color_curve(Brushes.Black.ToString());
                cp = 0;
            }

            else
            {


                MessageBox.Show("Le segment existe deja dans la zone de dessin", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        bool show;
        private void listViewItem4_Selected(object sender, RoutedEventArgs e)
        {
            /**Cette méthode est liéee au button profil du menu **/
            try
            {
                distances = new List<double>();

                // trier les coordonnees  avec la méthode de bulle selon l'absicess x en allant de gauche à droite
                int K = 0, I, J;
                for (K = 0; K < coord.Count; K++)
                {

                    for (I = coord.Count - 2; I >= 0; I--)
                    {
                        for (J = 0; J <= I; J++)
                        {
                            if (coord[J + 1].X < coord[J].X)
                            {
                                Point t = coord[J + 1];
                                coord[J + 1] = coord[J];
                                coord[J] = t;
                                int alt = altitudes[J + 1];
                                altitudes[J + 1] = altitudes[J];
                                altitudes[J] = alt;
                            }
                        }
                    }
                }

                double Prec = 0;
                for (K = 0; K < coord.Count - 1; K++)
                { // convertir les coordonnées obtenus en pixel en coordonnées réeeles 
                    double res = Math.Pow((coord[K].X - coord[K + 1].X), 2.0) + Math.Pow((coord[K].Y - coord[K + 1].Y), 2.0);
                    double distance = Math.Sqrt(res);
                    //distance = distance ;
                    distance = distance + Prec;
                    if (K == coord.Count - 2)
                    {
                        dxmax = distance; // la taille de l'axe x du graphe
                    }
                    Prec = distance;
                    distances.Add(PxToCm(distance));


                }

                for (K = 0; K < distances.Count; K++)
                { // multiplié les distances par l'échelle
                    distances[K] = distances[K] * echell[0];
                }
                List<point> liste = new List<point>();
                liste.Add(new point(0, altitudes[0]));
                for (K = 0; K < distances.Count; K++)
                {
                    liste.Add(new point(distances[K], altitudes[K + 1]));
                }
                // choisir les couleurs du graphe
                Color cl = (Color)ColorConverter.ConvertFromString("#FFA52A2A");
                Color back = (Color)ColorConverter.ConvertFromString("#FFE6E6FA");

                // on fait appel au constructeur profil
                Profil2 profil = new Profil2(distances[distances.Count - 1], minimum[0] - equidis[0], maximum[0] + equidis[0], dxmax + 80, equidis[0], liste, Unite[0], Uniteal[0], cl, back);

                allData.Add(new Tuple<List<Point>, List<int>>(coord, altitudes));
                profil.ShowActivated = true;
                profil.ShowDialog();
                profil_click.IsChecked = false;

            }
            // cette exception sera utilisé pour éviter que l'utilisateur clique sur le profil avant l'intersection
            catch (Exception ex)
            {
                MessageBox.Show("Tracer le segment puis cliquer sur le bouton intersection");
                profil_click.IsChecked = false;
            }

        }

        //appel à la classe help pour afficher l'aide
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Help y = new Help();
            // InitialiseTable();
            y.Show();
            // this.Content = y;



        }


        private void dessin_Checked(object sender, RoutedEventArgs e)
        {

        }



        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void listViewItem5_Selected(object sender, RoutedEventArgs e)
        {
            if (intersection_button.IsChecked == true) { intersection(sender, e); }
            else
            {
                foreach (Ellipse ell in intersection_ellipses)

                {
                    canvas.Children.Remove(ell);

                }
            }
        }
        //pour imprimer le travail
        private void ButtonImprimer_Click(object sender, RoutedEventArgs e)
        {

            PrintDialog printDialog = new PrintDialog();// appel à la fenetre d'impression
            if (printDialog.ShowDialog() == true)
            {

                printDialog.PrintVisual(src, "Printing");//la méthode d'impression
            }

        }
        /*    private void dessiner(object sender, RoutedEventArgs e)
            {
                Curve curv1 = new Curve();
                curv1.inc_numordre();
                curv = curv1;
                draw = 1;

            }*/

        private bool IsInt(string sVal)
        {
            int cpt = 1;
            foreach (char c in sVal)
            {
                int iN = (int)c;
                if ((iN > 57) || (iN < 48))
                {
                    if ((cpt != 1) || (iN != 45))
                    {
                        return false;
                    }
                }
                cpt++;
            }
            return true;
        }


        // right click




private void new_draw(object sender, MouseButtonEventArgs e)
        {
            if (draw == 1)

            {
                foreach (Ellipse r in circle_list)
                {
                    canvas.Children.Remove(r);

                }
                circle_list.Clear();
                if (curv.Cpt > 1) { canvas.Children.Remove(curv.Seg.Line); }
                var values = typeof(Brushes).GetProperties().Select(p => new { Name = p.Name, Brush = p.GetValue(null) as Brush }).ToArray();
                var brushNames = values.Select(v => v.Name).ToArray();
                bool exept = false;
                bool entre = false;
                int spec = 0;

                while (exept == false)
                {
                    Input_altitude4 altitude = new Input_altitude4("altitude", "");
                    if (altitude.ShowDialog() == true)
                    {
                        if (IsInt(altitude.Answer) == false)
                        {
                            MessageBox.Show("Vouz devez entrer un entier");
                            altitude.Close();
                        }
                        else
                        {

                            if ((Int32.Parse(altitude.Answer) - minimum[0]) % equidis[0] != 0)
                            {
                                spec = Int32.Parse(altitude.Answer);
                                MessageBox.Show("l'altitude ne peut pas prendre cette valeur");


                                MessageBox.Show("Veuillez s'il vous plait indiquez s'il s'agit d'un point sommet ou bas en répandant dans la fenetre qui sera affichée par oui ou non" +
                                    "SVP ce point doit etre entre le (min) et le (max) ");
                                Input_altitude4 ouinon = new Input_altitude4("oui/non", "");
                                if (ouinon.ShowDialog() == true)
                                {
                                    if (ouinon.Answer == "oui")
                                    {
                                        if (Int32.Parse(altitude.Answer) > maximum[0] || Int32.Parse(altitude.Answer) < minimum[0])
                                        {
                                            MessageBox.Show("L'altitude doit etre entre le min et le max");
                                            altitude.Close();
                                        }
                                        else
                                        {
                                            exept = true;
                                            entre = true;
                                            MessageBox.Show("Votre point a été enregidtré");
                                        }

                                    }
                                    else
                                    {
                                        altitude.Close();
                                    }

                                }
                            }

                            else
                            {
                                if (Int32.Parse(altitude.Answer) % 5 != 0)
                                {
                                    MessageBox.Show("L'altitude doit etre multiple de 5");
                                    altitude.Close();
                                }
                                else
                                {
                                    if (Int32.Parse(altitude.Answer) > maximum[0] || Int32.Parse(altitude.Answer) < minimum[0])
                                    {
                                        MessageBox.Show("L'altitude doit etre entre le min et le max");
                                        altitude.Close();
                                    }
                                    else
                                    {

                                        curv.Altitude = Int32.Parse(altitude.Answer);


                                        exept = true;
                                    }

                                }
                            }
                        }
                    }
                }

                if (entre)
                {
                    curv.Altitude = spec;


                    exept = true;
                }

                stop = false;
                foreach (Curve cur in curvelist)
                {
                    if (cur.Altitude == curv.Altitude)
                    {
                        stop = true;
                        curv.color_curve(cur.Colorname);
                    }
                }

                if (stop == false)
                {
                    curv.color_curve(brushNames[color]);
                    color = color + 6;

                }

                curvelist.Add(curv);
                foreach (Ellipse ell in intersection_ellipses)

                {
                    canvas.Children.Remove(ell);

                }
                listViewItem5_Selected(sender, e);
                Curve curv2 = new Curve();
                curv2.inc_numordre();
                curv = curv2;
            }

        }


     


        // left click



        private void drawing(object sender, MouseButtonEventArgs e)
        {

            if (draw == 1 || ((draw == 2) && (cp < 2)))
            {
                circle_list.AddRange(curv.Draw_curve_leftclick(new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y), canvas));//draw=1 indique qu'on va dessiner les points composants une courbe 

            }
            if (((draw == 2) && (cp < 2)))
            {
                circle_list.AddRange(SegmentAB.Draw_curve_leftclick(new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y), canvas));//draw=2 indique qu'on va dessiner les deux points compsants le segment AB
                cp++;
                if (cp == 2)//cp =2 indique qu'on a dessiner le deuxieme point du segment AB
                {
                    foreach (Ellipse r in circle_list)
                    {
                        canvas.Children.Remove(r);

                    }
                    circle_list.Clear();
                }
            }



        }

















        private void dessiner_segment(object sender, RoutedEventArgs e) //Initialiser le segment AB
        {
            if ((SegmentAB.SegmentList.Count == 0))  //Si le segment n'existe pas dans la zone de dessin
            {
                foreach (Ellipse ell in circle_list)
                {
                    canvas.Children.Remove(ell);
                }
                circle_list.Clear();
                draw = 2;
                SegmentAB = new Curve();
                SegmentAB.color_curve(Brushes.Black.ToString());//Affecter une couleur noire au segment
                cp = 0;
            }

            else
            {

                MessageBox.Show("Le segment existe deja dans la zone de dessin", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }



        private void Grid_MouseMove(object sender, RoutedEventArgs e)
        {

        }
        private void Cut(object sender, RoutedEventArgs e)
        {
            if (selectedcurve == null) return;
            //int p = 0;
            cut = true;
            copiedcurve = selectedcurve;
            foreach (Ellipse r in circle_list)
            {
                canvas.Children.Remove(r);
            }
            circle_list.Clear();

            curvelist.RemoveAt(indexselected);

            /*supprimer les segments du canvas */
            foreach (Segment seg in selectedcurve.SegmentList)
            {
                canvas.Children.Remove(seg.Line);
            }

            selectedcurve = null;
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            if (selectedcurve == null) return;
            cut = false;
            copiedcurve = selectedcurve;
            selectedcurve = null;
        }

        private void MouseDownModify(MouseButtonEventArgs e)
        {
            object TestPanelOrUI = this.canvas.InputHitTest(e.GetPosition(this.canvas)) as FrameworkElement;//test the element we clicked on
            if (TestPanelOrUI != null)
            {
                indexselected = 0;
                if (TestPanelOrUI is Polyline || TestPanelOrUI is Ellipse)
                {
                    if (TestPanelOrUI is Polyline) // if it's a curve
                    {
                        supp2 = true; suppr = false;
                        if ((ajouter == true) && (trouv == true) && (mouve == true) && (trouvAB == true))/*Ajouter point sur le segment d'une courbe*/
                        {
                            circle_list.Add(selectedcurve.ajout_point_leftclick(new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y), canvas, seg_mouve));
                            //selectedcurve.color_curve(selectedcurve.Colorname);

                            Curve courbe = new Curve();
                            int k = 0;
                            // courbe.SegmentList.AddRange(copiedcurve.SegmentList);
                            courbe.Altitude = selectedcurve.Altitude;
                            courbe.Pointdebut = selectedcurve.Pointdebut;
                            courbe.Liste_circle.AddRange(selectedcurve.Liste_circle);
                            courbe.Colorname = selectedcurve.Colorname;
                            courbe.Cpt = selectedcurve.Cpt;
                            courbe.Get_numordre = selectedcurve.Get_numordre;
                            curvelist.Add(courbe);
                            curvelist.Remove(selectedcurve);
                            foreach (Segment seg in selectedcurve.SegmentList)
                            {
                                Segment sg = new Segment();
                                sg.A = seg.A;
                                sg.B = seg.B;
                                courbe.SegmentList.Add(sg);
                                sg.Draw_segment(canvas);
                                canvas.Children.Remove(seg.Line);
                            }
                            foreach (Ellipse ell in selectedcurve.Liste_circle)
                            {
                                canvas.Children.Remove(ell);

                                canvas.Children.Add(ell);


                            }
                            courbe.color_curve(courbe.Colorname);
                            listViewItem5_Selected(intersection_button, e);
                            ajouter = false;
                            foreach (Ellipse ell in circle_list)
                            {
                                canvas.Children.Remove(ell);
                            }
                            circle_list.Clear();

                            selectedpoint = null;
                            selectedline = null;
                            selectedcurve = null;
                            indexselected = 0;
                            indexpointselected = 0;


                        }

                        else
                        {

                            if (selectedline == null)  //selectedline est vide au debut
                            {
                                Draw_points((Polyline)TestPanelOrUI);
                                selectedline = (FrameworkElement)TestPanelOrUI;
                                mouve = true;
                            }
                            else if (selectedline.Equals((FrameworkElement)TestPanelOrUI) == false)  //selectedline pointait une autre courbe deja  
                            {
                                //removethe control point and the tangents for the las selected curve
                                foreach (Ellipse ell in circle_list) //Supprimer les points deja existants sur Canvas de l'ancienne courbe 
                                {
                                    canvas.Children.Remove(ell);
                                }
                                circle_list.Clear();


                                mouve = true;
                                selectedline = (FrameworkElement)TestPanelOrUI;

                                Draw_points((Polyline)TestPanelOrUI);


                            }

                        }
                    }

                    else if (TestPanelOrUI is Ellipse) // if it's a control point
                    {
                        if (Find_Ellipse((Ellipse)TestPanelOrUI) == true)
                        {
                            selectedpoint = (Ellipse)TestPanelOrUI;
                            if (e.ClickCount == 2)//indique qu'on va bouger le point 
                            {
                                pointmoved = true;

                            }
                            if (e.ClickCount == 1)//imdique qu'on va supprimer le point
                            {
                                pointmoved = false;
                                suppr = true;
                            }

                        }
                        else
                        {
                            pointmoved = false; suppr = false; selectedpoint = null; indexpointselected = 0;
                        }
                    }
                }
                else //si l'utilisateur clique sur Canvas 
                {



                    if ((ajouter == true) && (trouv == true) && (mouve == true) && (trouvAB == true)) //ajouter un point sur le canvas 
                    {
                        circle_list.Add(selectedcurve.ajout_point_leftclick(new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y), canvas, seg_mouve));
                        //selectedcurve.color_curve(selectedcurve.Colorname);

                        Curve courbe = new Curve();
                        int k = 0;
                        // courbe.SegmentList.AddRange(copiedcurve.SegmentList);
                        courbe.Altitude = selectedcurve.Altitude;
                        courbe.Pointdebut = selectedcurve.Pointdebut;
                        courbe.Liste_circle.AddRange(selectedcurve.Liste_circle);
                        courbe.Colorname = selectedcurve.Colorname;
                        courbe.Cpt = selectedcurve.Cpt;
                        courbe.Get_numordre = selectedcurve.Get_numordre;
                        curvelist.Add(courbe);
                        curvelist.Remove(selectedcurve);
                        foreach (Segment seg in selectedcurve.SegmentList)
                        {
                            Segment sg = new Segment();
                            sg.A = seg.A;
                            sg.B = seg.B;
                            courbe.SegmentList.Add(sg);
                            sg.Draw_segment(canvas);
                            canvas.Children.Remove(seg.Line);
                        }
                        foreach (Ellipse ell in selectedcurve.Liste_circle)
                        {
                            canvas.Children.Remove(ell);

                            canvas.Children.Add(ell);


                        }
                        courbe.color_curve(courbe.Colorname);
                        foreach (Ellipse el in intersection_ellipses)
                        {
                            canvas.Children.Remove(el);
                        }
                        listViewItem5_Selected(intersection_button, e);
                        ajouter = false;


                    }

                    foreach (Ellipse ell in circle_list) //Supprimer les points rouges indiquant que la courbe est selectionnee
                    {
                        canvas.Children.Remove(ell);
                    }
                    circle_list.Clear();

                    selectedpoint = null;
                    selectedline = null;
                    selectedcurve = null;
                    indexselected = 0;
                    indexpointselected = 0;


                }
            }
        }


        public bool Find_Ellipse(Ellipse elps)/*Rechercher dans la liste des points composants la courbe selectionnee le point selectionne par la souris et retourner son index */
        {
            indexpointselected = 0;
            bool trouv = false;
            if (selectedcurve != null)
            {
                foreach (Ellipse c in selectedcurve.Liste_circle)
                {
                    if (trouv == false)
                    {
                        if (elps.Equals(c))
                        {
                            trouv = true;
                        }

                        indexpointselected++;
                    }

                }

            }
            else
            {
                trouv = false;
            }
            return trouv;
        }




        private void Draw_points(Polyline line)/*Dessiner des points rouges sur les zones d'intersection des lignes composantes la courbe apres l'avoir selectionne */
        {
            int k = 0;
            trouv = false;
            selected = 0;
            foreach (Curve c in curvelist)
            {
                foreach (Segment seg in c.SegmentList)
                {
                    if (seg.Line.Equals(line))
                    {
                        trouvAB = true;
                        trouv = true;
                        selectedcurve = c;// contient la courbe selectionne
                        indexselected = k; //indique la position de la courbe selectionnee dans curvelist
                        seg_mouve = seg;//contient le segment selectionne 

                        foreach (Ellipse ell in c.Liste_circle)
                        {
                            canvas.Children.Add(ell);
                            circle_list.Add(ell);



                        }

                    }

                }
                if (trouv == false)
                {

                    k++;
                }
            }
            if (trouv == false)
            {
                selectedcurve = SegmentAB; //Si la courbe selectionne ne se trouve pas dans la liste des courbes alors c'est le segment AB 
                trouvAB = false;
                foreach (Ellipse ell in SegmentAB.Liste_circle)
                {
                    canvas.Children.Add(ell);
                    circle_list.Add(ell);
                    trouv = true;
                }

            }
        }

        private void Draw_Click(object sender, MouseButtonEventArgs e)
        {

            if (dessin.IsChecked == true)//Si togglebouton est allume on peut effectuer un dessin sur Canvas
            {
                drawing(sender, e);
            }
            else //Sinon,On peut selectionner soit le sefment AB soit l'une des courbes
            {
                MouseDownModify(e);
            }

        }

        /* private void setting_MouseDown(object sender, MouseButtonEventArgs e)
         {
             if (e.ChangedButton==MouseButton.Left)
             {
                 Button button = sender as Button;
                 ContextMenu contextMenu = button.ContextMenu;
                 contextMenu.PlacementTarget = button;
                 contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                 contextMenu.IsOpen = true;
                 e.Handled = true;
             }
         }*/

        public void
            SaveThings(String path)
        {

            XmlSerializer xmlSeries = new XmlSerializer(typeof(SaveFormat));
            //List<Tuple < List < Segment >, List<int>>> cord = new List<Tuple<List<Segment>, List<int>>>();
            List<List<Point>> sgmts = new List<List<Point>>();
            List<List<int>> alts = new List<List<int>>();
            List<List<int>> alt = new List<List<int>>();
            List<List<String>> clrs = new List<List<String>>();
            List<List<String>> clr = new List<List<String>>();
            List<Point> sgmtAB = new List<Point>();
            Point p = new Point();
            SaveFileDialog parcourir = new SaveFileDialog();
            String image = "";
            if (SegmentAB.SegmentList.Count != 0)
            {
                sgmtAB.Add(SegmentAB.SegmentList.First().A);
                sgmtAB.Add(SegmentAB.SegmentList.First().B);

            }
            else
            {
            }


            image = ImgOriginal.Source.ToString();
            foreach (Curve el in curvelist)
            {
                List<int> lt = new List<int>();
                List<String> cl = new List<String>();
                lt.Add(el.Altitude);
                alt.Add(lt);
                cl.Add(el.Colorname);
                clr.Add(cl);

                List<Point> cor = new List<Point>();
                foreach (Segment atb in el.SegmentList)

                {
                    cor.Add(atb.A);
                    // cor.Add(atb.B);
                    p = atb.B;
                }
                cor.Add(p);
                sgmts.Add(cor);
            }
            foreach (List<int> al in alt)
            {
                alts.Add(al);
            }
            foreach (List<String> c in clr)
            {
                clrs.Add(c);
            }
            System.IO.File.WriteAllText(path, string.Empty);
            using (System.IO.TextWriter writer = new System.IO.StreamWriter(path))
            {

                xmlSeries.Serialize(writer, new SaveFormat(image, sgmts, alts, sgmtAB, clrs, allpente.First().Item1, allpente.First().Item2, allpente.First().Item3, allpente.First().Item4, allpente.First().Item5, allpente.First().Item6, color));

            }

        }
        public void LoadThings(string path)
        {//Load a saved file from a path
            intersection_points.Clear();
            circle_list.Clear();
            curvelist.Clear();
            suppr = false;
            supp2 = false;
            pointmoved = false;
            ajouter = false; mouve = false;


            indexpointselected = 0;

            if (SegmentAB.SegmentList.Count != 0)
            {
                canvas.Children.Remove(SegmentAB.SegmentList.First().Line);
                SegmentAB.SegmentList.Clear();
                SegmentAB.Liste_circle.Clear();
                SegmentAB.Liste_circle2.Clear();



            }

            SegmentAB = new Curve();
            maximum.Clear();
            minimum.Clear();
            equidis.Clear();
            echell.Clear();
            Unite.Clear();
            Uniteal.Clear();
            SaveFormat fileData;
            XmlSerializer xmlSeries = new XmlSerializer(typeof(SaveFormat));
            using (System.IO.TextReader reader = new System.IO.StreamReader(path))
            {
                fileData = (SaveFormat)xmlSeries.Deserialize(reader);
            }

            //allData.Clear();

            canvas.Children.Clear();
            canvas.Children.Add(ImgOriginal);
            ImgOriginal.Width = 693;
            ImgOriginal.Height = 428;
            maximum.Add(fileData.MAX);
            minimum.Add(fileData.MIN);
            equidis.Add(fileData.Equidistance);
            echell.Add(fileData.Echelle);
            color = fileData.Color;
            Unite.Add(fileData.METRE);
            Uniteal.Add(fileData.EchelleY);


            var img = fileData.image;
            ImgOriginal.Source = new BitmapImage(new Uri(@img));
            allpente.Clear();
            allpente.Add(new Tuple<int, int, int, double, String, String>(fileData.MAX, fileData.MIN, fileData.Equidistance, fileData.Echelle, fileData.METRE, fileData.EchelleY));
            //List<Curve> nvcourbe = new List<Curve>();

            Curve sgab = new Curve();
            int k = 0;
            foreach (List<Point> el in fileData.segments)
            {
                List<Segment> s = new List<Segment>();
                Curve crv = new Curve();
                int cpt = 0;
                while (cpt < (el.Count - 1))
                {


                    Segment ab = new Segment();
                    ab.A = el[cpt];
                    ab.B = el[cpt + 1];
                    Ellipse circle = new Ellipse()
                    {
                        Width = 6,
                        Height = 6,
                        Stroke = Brushes.Red,
                        StrokeThickness = 6
                    };


                    circle.SetValue(Canvas.LeftProperty, (ab.A.X - 3));
                    circle.SetValue(Canvas.TopProperty, (ab.A.Y - 3));
                    crv.Liste_circle.Add(circle);
                    //circle_list.Add(circle);  //-----
                    if (cpt == el.Count - 2) //22
                    {
                        circle = new Ellipse()
                        {
                            Width = 6,
                            Height = 6,
                            Stroke = Brushes.Red,
                            StrokeThickness = 6
                        };
                        circle.SetValue(Canvas.LeftProperty, (ab.B.X - 3));
                        circle.SetValue(Canvas.TopProperty, (ab.B.Y - 3));
                        crv.Liste_circle.Add(circle);
                        // circle_list.Add(circle);  //-----
                    }
                    s.Add(ab);
                    cpt++;

                    //canvas.Children.Remove(ab.Line);
                    ab.Draw_segment(canvas);
                }

                crv.SegmentList.AddRange(s);
                crv.Altitude = fileData.altitude[k].First();
                crv.Colorname = fileData.couleurs[k].First();
                crv.color_curve(crv.Colorname);
                curvelist.Add(crv);
                k++;

            }
            Segment abb = new Segment();
            if (fileData.segmentAB.Count != 0)
            {
                abb.A = fileData.segmentAB[0];
                abb.B = fileData.segmentAB[1];
                SegmentAB.SegmentList.Add(abb);
                Curve CRVAB = new Curve();
                Ellipse circle = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };
                circle.SetValue(Canvas.LeftProperty, (abb.A.X - 3));
                circle.SetValue(Canvas.TopProperty, (abb.A.Y - 3));
                SegmentAB.Liste_circle.Add(circle);
                Ellipse circle2 = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };
                circle2.SetValue(Canvas.LeftProperty, (abb.B.X - 3));
                circle2.SetValue(Canvas.TopProperty, (abb.B.Y - 3));
                SegmentAB.Liste_circle.Add(circle2);
                // curvelist.Add(SegmentAB);
                SegmentAB.SegmentList.First().Draw_segment(canvas);
                SegmentAB.color_curve(Brushes.Black.ToString());
            }






            /* if (SegmentAB.SegmentList.Count != 0)
             {
                 canvas.Children.Remove(SegmentAB.SegmentList.First().Line);
                 SegmentAB.SegmentList.First().Draw_segment(canvas);
             }*/


        }
        // pour ouvrir le paramètre de l'application
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("parametre") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;

        }
        // pour changer la couleur de l'application en rouge
        private void red_click(object sender, RoutedEventArgs e)
        {

            menuitem.Background = Brushes.Green;
            test.Background = Brushes.Green;
            toto.Foreground = Brushes.Green;
            ButtonOpenMenu.Foreground
                = Brushes.Green;
        }
        /* private void ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
         {
             if (ImgOriginal != null)
             {
                 double scale = (double)(Zoom.Value / 100.0);
                 ImgOriginal.LayoutTransform = new ScaleTransform(scale, scale);

             }

         }*/
        // pour changer la couleur de l'application en vert
        private void green_click(object sender, RoutedEventArgs e)
        {

            menuitem.Background = Brushes.Red;
            test.Background = Brushes.Red;
            toto.Foreground = Brushes.Red;
            ButtonOpenMenu.Background = Brushes.Red;
        }
        // pour changer la couleur de l'application en bleu
        private void bleu_click(object sender, RoutedEventArgs e)
        {

            menuitem.Background = Brushes.Blue;
            test.Background = Brushes.Blue;
            toto.Foreground = Brushes.Blue;
            ButtonOpenMenu.Background = Brushes.Blue;
        }
        // pour changer la couleur de l'application en marron
        private void marron_click(object sender, RoutedEventArgs e)
        {

            menuitem.Background = Brushes.Brown;
            test.Background = Brushes.Brown;
            toto.Foreground = Brushes.Brown;
            ButtonOpenMenu.Background = Brushes.Brown;
        }
        // pour changer la couleur de l'application en vert
        private void gris_click(object sender, RoutedEventArgs e)
        {

            menuitem.Background = Brushes.Gray;
            test.Background = Brushes.Gray;
            toto.Foreground = Brushes.Gray;
            ButtonOpenMenu.Background = Brushes.Gray;
        }
        // pour changer la couleur de l'application en bleuvert
        private void bleu2_click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush my = new SolidColorBrush();
            my.Color = Color.FromRgb(50, 200, 180);
            menuitem.Background = my;
            test.Background = my;
            toto.Foreground = my;
            ButtonOpenMenu.Background = my;
        }
        // pour changer la couleur de l'application en noir
        private void noir_click(object sender, RoutedEventArgs e)
        {

            menuitem.Background = Brushes.Black;
            test.Background = Brushes.Black;
            toto.Foreground = Brushes.Black;
            ButtonOpenMenu.Background = Brushes.Black;
        }
        // pour  retourner à la couleur initiale de l'application
        private void origine_click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush my = new SolidColorBrush();
            my.Color = Color.FromRgb(53, 128, 191);
            menuitem.Background = my;
            test.Background = my;
            toto.Foreground = my;
            ButtonOpenMenu.Background = my;
        }
        // pour changer la couleur de l'application en orange
        private void orange_click(object sender, RoutedEventArgs e)
        {

            menuitem.Background = Brushes.Orange;
            test.Background = Brushes.Orange;
            toto.Foreground = Brushes.Orange;
            ButtonOpenMenu.Background = Brushes.Orange;
        }

        private void copy_click(object sender, RoutedEventArgs e)
        {

            if ((selectedcurve == null) || (selectedcurve == null)) return;
            cut = false;
            copiedcurve = selectedcurve;
            selectedcurve = null;

        }
        private void paste_click(object sender, RoutedEventArgs e)
        {


            if (copiedcurve == null) return;
            if (cut)
            {
                /*  foreach (Segment seg in copiedcurve.SegmentList)
                  {
                      Segment s = new Segment();
                      s.A = seg.A;
                      s.B = seg.B;
                      s.Draw_segment(canvas);

                  }
                  curvelist.Add(copiedcurve);*/
            }


            Curve courbe = new Curve();
            int k = 0;

            courbe.Altitude = copiedcurve.Altitude;
            courbe.Pointdebut = copiedcurve.Pointdebut;
            courbe.Colorname = copiedcurve.Colorname;
            courbe.Cpt = copiedcurve.Cpt;
            copiedcurve.inc_numordre();
            curvelist.Add(courbe);

            foreach (Segment seg in copiedcurve.SegmentList)
            {
                Segment sg = new Segment();
                sg.A = new Point(seg.A.X + 20, seg.A.Y + 20);
                sg.B = new Point(seg.B.X + 20, seg.B.Y + 20);
                courbe.SegmentList.Add(sg);
                sg.Draw_segment(canvas);
                Ellipse circle = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };

                k++;
                circle.SetValue(Canvas.LeftProperty, (sg.A.X - 3));
                circle.SetValue(Canvas.TopProperty, (sg.A.Y - 3));
                courbe.Liste_circle.Add(circle);

                if (k == copiedcurve.SegmentList.Count)
                {
                    circle = new Ellipse()
                    {
                        Width = 6,
                        Height = 6,
                        Stroke = Brushes.Red,
                        StrokeThickness = 6
                    };
                    circle.SetValue(Canvas.LeftProperty, (sg.B.X - 3));
                    circle.SetValue(Canvas.TopProperty, (sg.B.Y - 3));
                    courbe.Liste_circle.Add(circle);
                }


            }
            courbe.color_curve(courbe.Colorname);


        }

        private void paste_executed(object sender, ExecutedRoutedEventArgs e)
        {

            if (copiedcurve == null) return;
            if (cut)
            {
                /*  foreach (Segment seg in copiedcurve.SegmentList)
                  {
                      Segment s = new Segment();
                      s.A = seg.A;
                      s.B = seg.B;
                      s.Draw_segment(canvas);

                  }
                  curvelist.Add(copiedcurve);*/
            }


            Curve courbe = new Curve();
            int k = 0;

            courbe.Altitude = copiedcurve.Altitude;
            courbe.Pointdebut = copiedcurve.Pointdebut;
            courbe.Colorname = copiedcurve.Colorname;
            courbe.Cpt = copiedcurve.Cpt;
            copiedcurve.inc_numordre();
            curvelist.Add(courbe);

            foreach (Segment seg in copiedcurve.SegmentList)
            {
                Segment sg = new Segment();
                sg.A = new Point(seg.A.X + 20, seg.A.Y + 20);
                sg.B = new Point(seg.B.X + 20, seg.B.Y + 20);
                courbe.SegmentList.Add(sg);
                sg.Draw_segment(canvas);
                Ellipse circle = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };

                k++;
                circle.SetValue(Canvas.LeftProperty, (sg.A.X - 3));
                circle.SetValue(Canvas.TopProperty, (sg.A.Y - 3));
                courbe.Liste_circle.Add(circle);

                if (k == copiedcurve.SegmentList.Count)
                {
                    circle = new Ellipse()
                    {
                        Width = 6,
                        Height = 6,
                        Stroke = Brushes.Red,
                        StrokeThickness = 6
                    };
                    circle.SetValue(Canvas.LeftProperty, (sg.B.X - 3));
                    circle.SetValue(Canvas.TopProperty, (sg.B.Y - 3));
                    courbe.Liste_circle.Add(circle);
                }


            }
            courbe.color_curve(courbe.Colorname);


        }
        //pour effectuer le raccourci clavier ctrl +v
        private void paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        //appel au méthode copier
        private void copy_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (selectedcurve == null) return;
            cut = false;
            copiedcurve = selectedcurve;
            selectedcurve = null;



        }
        // pour effectuer le raccourci clavier ctrl+c
        private void copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void add_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ajouter = true;
        }

        //appel à la méthode couper
        private void cut_executed(object sender, ExecutedRoutedEventArgs e)
        {

            if (selectedcurve == null) return;
            //int p = 0;
            cut = true;
            copiedcurve = selectedcurve;
            foreach (Ellipse r in circle_list)
            {
                canvas.Children.Remove(r);
            }
            circle_list.Clear();

            curvelist.RemoveAt(indexselected);

            /*supprimer les segments du canvas */
            foreach (Segment seg in selectedcurve.SegmentList)
            {
                canvas.Children.Remove(seg.Line);
            }

            selectedcurve = null;


        }
        // pour effectuer le raccourci clavier ctrl+x
        private void cut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void add_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void cut_click(object sender, RoutedEventArgs e)
        {

            if ((selectedcurve == null) || (selectedcurve == null)) return;
            //int p = 0;
            cut = true;
            copiedcurve = selectedcurve;
            foreach (Ellipse r in circle_list)
            {
                canvas.Children.Remove(r);
            }
            circle_list.Clear();

            curvelist.RemoveAt(indexselected);

            /*supprimer les segments du canvas */
            foreach (Segment seg in selectedcurve.SegmentList)
            {
                canvas.Children.Remove(seg.Line);
            }

            selectedcurve = null;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {//handles click
            if (dessin.IsChecked == true)
            {
                new_draw(sender, e);
            }
            else
            {
                deplacercount(e);
            }

        }

        private void deplacercount(MouseButtonEventArgs e)
        {
            if (mouved == true)
            {
                mouved = false;
            }
        }
        //pour maximiser et minimiser l'application
        private void ButtonMaximise_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                GridMain.Width = 1365;

                //ImgOriginal.Height = 800;
                this.WindowState = WindowState.Maximized;
                GridMain.Height = 900;
                src.Width = 1024;
                src.Height = 820;
                canvas.Width = 1024;
                canvas.Height = 820;

                ImgOriginal.Width = 1015;
                //// sliZoom.Value = 130;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                // this.Width = SystemParameters.WorkArea.Width;
                //this.Height = SystemParameters.WorkArea.Height;
                this.WindowState = WindowState.Normal;
                GridMain.Width = 1024;
                GridMain.Height = 600;
                src.Width = 713;
                canvas.Width = 700;
                ImgOriginal.Width = 690;
                sliZoom.Value = 100;
            }

        }
        //pour cacher l'application dans la barre des taches
        private void ButtonMinimise_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }
        // pour zoomer l'application
        private void sliZoom_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            // Make sure the control's are all ready.
            if (!IsInitialized) return;


            // if (ImgOriginal != null)
            //{
            // Get the scale factor as a fraction 0.25 - 2.00.
            double scale = (double)(sliZoom.Value / 100.0);

            // Scale the graph.
            canvas.LayoutTransform = new ScaleTransform(scale, scale);
            ImgOriginal.LayoutTransform = new ScaleTransform(scale, scale);
            //}
        }



        private void supprimer_point(object sender, RoutedEventArgs e)
        {
            if (suppr == true)
            {
                pointmoved = false;



                if (indexpointselected != 1) //Si le point selectionne n'est pas le poni debut de la courbe 
                {

                    if (selectedcurve.Liste_circle.Count == indexpointselected) //Si le point selectionne est le dernier composant la courbe
                    {
                        canvas.Children.Remove(selectedcurve.SegmentList[indexpointselected - 2].Line);
                        canvas.Children.Remove(selectedcurve.Liste_circle[indexpointselected - 1]);
                        selectedcurve.SegmentList.Remove(selectedcurve.SegmentList[indexpointselected - 2]);
                        selectedcurve.Liste_circle.Remove(selectedcurve.Liste_circle[indexpointselected - 1]);
                        circle_list.Remove(circle_list[indexpointselected - 1]);
                        if (selectedcurve.Liste_circle.Count == 1)
                        {
                            canvas.Children.Remove(selectedcurve.Liste_circle[indexpointselected - 2]);
                            curvelist.Remove(selectedcurve);
                            circle_list.Clear();
                            supp2 = false;
                        }


                    }
                    else // Si le point selectionne se trouve au milieu de la courbe
                    {
                        Point A = selectedcurve.SegmentList[indexpointselected - 2].A;
                        Point B = selectedcurve.SegmentList[indexpointselected - 1].B;
                        canvas.Children.Remove(selectedcurve.SegmentList[indexpointselected - 2].Line);
                        canvas.Children.Remove(selectedcurve.SegmentList[indexpointselected - 1].Line);
                        canvas.Children.Remove(selectedcurve.Liste_circle[indexpointselected - 1]);
                        selectedcurve.SegmentList.Remove(selectedcurve.SegmentList[indexpointselected - 2]);
                        selectedcurve.SegmentList.Remove(selectedcurve.SegmentList[indexpointselected - 2]);
                        selectedcurve.Liste_circle.Remove(selectedcurve.Liste_circle[indexpointselected - 1]);

                        circle_list.Remove(circle_list[indexpointselected - 1]);
                        seg = new Segment();
                        seg.A = A; seg.B = B;
                        seg.Draw_segment(canvas);
                        seg.color_segment(selectedcurve.Colorname);
                        selectedcurve.SegmentList.Insert(indexpointselected - 2, seg);
                    }

                    foreach (Ellipse ell in circle_list)
                    {
                        canvas.Children.Remove(ell);
                    }
                    foreach (Ellipse ell in circle_list)
                    {
                        canvas.Children.Add(ell);
                    }
                }

                suppr = false;
                indexpointselected = 0;








            }

            else if (supp2 == true) // indique qu'on va supprimer une courbe ou le segment AB
            {
                pointmoved = false;
                foreach (Segment sg in selectedcurve.SegmentList)
                {
                    canvas.Children.Remove(sg.Line);
                }
                foreach (Ellipse ell in selectedcurve.Liste_circle)
                {
                    canvas.Children.Remove(ell);
                }
                circle_list.Clear();

                if (SegmentAB == selectedcurve)
                {
                    SegmentAB.SegmentList.Clear();
                    SegmentAB.Liste_circle.Clear();
                    SegmentAB = new Curve();
                }
                else
                {
                    curvelist.RemoveAt(curvelist.IndexOf(selectedcurve));
                }
                supp2 = false;

            }
            foreach (Ellipse ell in intersection_ellipses)

            {
                canvas.Children.Remove(ell);

            }

            listViewItem5_Selected(sender, e);
            suppr = false;

        }



        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {


            if (e.LeftButton == MouseButtonState.Released) // On bouge la souris sur Canvas 
            {
                if (draw == 1) // On dessine une ligne composante le courbe
                {
                    curv.Draw_curve_mousemove(new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y), canvas);
                }
                if (((draw == 2) && (cp < 2)))// On dessine une ligne composante le segment AB
                {
                    SegmentAB.Draw_curve_mousemove(new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y), canvas);

                }

                if ((mouved == true) && (trouv == true) && (trouvAB == true)) //On va deplacer la courbe sur Canvas 
                {




                    foreach (Ellipse ell in intersection_ellipses)

                    {
                        canvas.Children.Remove(ell);

                    }

                    curvelist.Remove(selectedcurve);
                    L2 = selectedcurve.mouve_curve_mousemove(new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y), canvas, seg_mouve);
                    seg_mouve = L2;

                    curvelist.Add(selectedcurve);
                    Curve courbe = new Curve();
                    int k = 0;

                    courbe.Altitude = selectedcurve.Altitude;
                    courbe.Pointdebut = selectedcurve.Pointdebut;
                    courbe.Colorname = selectedcurve.Colorname;
                    courbe.Cpt = selectedcurve.Cpt;
                    selectedcurve.inc_numordre();
                    curvelist.Remove(selectedcurve);
                    foreach (Segment seg in selectedcurve.SegmentList)
                    {

                        canvas.Children.Remove(seg.Line);
                    }
                    curvelist.Add(courbe);

                    foreach (Segment seg in selectedcurve.SegmentList)
                    {
                        Segment sg = new Segment();
                        sg.A = new Point(seg.A.X, seg.A.Y);
                        sg.B = new Point(seg.B.X, seg.B.Y);
                        courbe.SegmentList.Add(sg);
                        sg.Draw_segment(canvas);
                        Ellipse circle = new Ellipse()
                        {
                            Width = 6,
                            Height = 6,
                            Stroke = Brushes.Red,
                            StrokeThickness = 6
                        };

                        k++;
                        circle.SetValue(Canvas.LeftProperty, (sg.A.X - 3));
                        circle.SetValue(Canvas.TopProperty, (sg.A.Y - 3));
                        courbe.Liste_circle.Add(circle);

                        if (k == selectedcurve.SegmentList.Count)
                        {
                            circle = new Ellipse()
                            {
                                Width = 6,
                                Height = 6,
                                Stroke = Brushes.Red,
                                StrokeThickness = 6
                            };
                            circle.SetValue(Canvas.LeftProperty, (sg.B.X - 3));
                            circle.SetValue(Canvas.TopProperty, (sg.B.Y - 3));
                            courbe.Liste_circle.Add(circle);
                        }


                    }
                    courbe.color_curve(courbe.Colorname);
                    selectedcurve = courbe;
                    // intersection_points.Clear();
                    if (intersection_button.IsChecked == true) { listViewItem5_Selected(sender, e); }
                }





            }



            if (pointmoved && mouved == false)
            {


                foreach (Ellipse ell in intersection_ellipses)

                {
                    canvas.Children.Remove(ell);

                }


                if ((indexpointselected < selectedcurve.Liste_circle.Count) || (indexpointselected == 1))
                {

                    Point B = selectedcurve.SegmentList[indexpointselected - 1].B;
                    canvas.Children.Remove(selectedpoint);
                    canvas.Children.Remove(selectedcurve.SegmentList[indexpointselected - 1].Line);
                    selectedcurve.SegmentList[indexpointselected - 1] = new Segment();
                    selectedcurve.SegmentList[indexpointselected - 1].A = new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y);
                    selectedcurve.SegmentList[indexpointselected - 1].B = B;
                    selectedcurve.SegmentList[indexpointselected - 1].color_segment(selectedcurve.Colorname);
                    selectedcurve.SegmentList[indexpointselected - 1].Draw_segment(canvas);

                }

                if ((indexpointselected > 1) && ((indexpointselected < selectedcurve.Liste_circle.Count)) || (indexpointselected == selectedcurve.Liste_circle.Count))
                {
                    Point A = selectedcurve.SegmentList[indexpointselected - 2].A;
                    canvas.Children.Remove(selectedpoint);
                    canvas.Children.Remove(selectedcurve.SegmentList[indexpointselected - 2].Line);
                    selectedcurve.SegmentList[indexpointselected - 2] = new Segment();
                    selectedcurve.SegmentList[indexpointselected - 2].A = A;
                    selectedcurve.SegmentList[indexpointselected - 2].B = new Point(e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y);
                    selectedcurve.SegmentList[indexpointselected - 2].color_segment(selectedcurve.Colorname);
                    selectedcurve.SegmentList[indexpointselected - 2].Draw_segment(canvas);

                }
                selectedpoint = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };

                circle_list[indexpointselected - 1] = selectedpoint;
                selectedcurve.Liste_circle[indexpointselected - 1] = selectedpoint;
                canvas.Children.Add(selectedpoint);
                selectedpoint.SetValue(Canvas.LeftProperty, (e.GetPosition(this.canvas).X - 3));
                selectedpoint.SetValue(Canvas.TopProperty, (e.GetPosition(this.canvas).Y - 3));

                foreach (Ellipse el in intersection_ellipses)
                {
                    canvas.Children.Remove(el);
                }

                listViewItem5_Selected(sender, e);

                if ((e.LeftButton == MouseButtonState.Pressed))


                    if (pointmoved == true)
                    {
                        pointmoved = false;
                        indexpointselected = 0;

                        /* curvelist.RemoveAt(curvelist.IndexOf(selectedcurve));
                         curvelist.Add(selectedcurve);*/



                        foreach (Ellipse ell in selectedcurve.Liste_circle)
                        {
                            if (!ell.Equals(selectedpoint))
                            {
                                canvas.Children.Remove(ell);
                            }
                        }



                        foreach (Ellipse ell in selectedcurve.Liste_circle)
                        {
                            if (!ell.Equals(selectedpoint))
                            {

                                canvas.Children.Add(ell);
                            }
                        }





                    }




            }


        }

        private void Paste(object sender, RoutedEventArgs e)
        {
            if (copiedcurve == null) return;
            if (cut)
            {
                /*  foreach (Segment seg in copiedcurve.SegmentList)
                  {
                      Segment s = new Segment();
                      s.A = seg.A;
                      s.B = seg.B;
                      s.Draw_segment(canvas);

                  }
                  curvelist.Add(copiedcurve);*/
            }


            Curve courbe = new Curve();
            int k = 0;

            courbe.Altitude = copiedcurve.Altitude;
            courbe.Pointdebut = copiedcurve.Pointdebut;
            courbe.Colorname = copiedcurve.Colorname;
            courbe.Cpt = copiedcurve.Cpt;
            copiedcurve.inc_numordre();
            curvelist.Add(courbe);

            foreach (Segment seg in copiedcurve.SegmentList)
            {
                Segment sg = new Segment();
                sg.A = new Point(seg.A.X + 20, seg.A.Y + 20);
                sg.B = new Point(seg.B.X + 20, seg.B.Y + 20);
                courbe.SegmentList.Add(sg);
                sg.Draw_segment(canvas);
                Ellipse circle = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };

                k++;
                circle.SetValue(Canvas.LeftProperty, (sg.A.X - 3));
                circle.SetValue(Canvas.TopProperty, (sg.A.Y - 3));
                courbe.Liste_circle.Add(circle);

                if (k == copiedcurve.SegmentList.Count)
                {
                    circle = new Ellipse()
                    {
                        Width = 6,
                        Height = 6,
                        Stroke = Brushes.Red,
                        StrokeThickness = 6
                    };
                    circle.SetValue(Canvas.LeftProperty, (sg.B.X - 3));
                    circle.SetValue(Canvas.TopProperty, (sg.B.Y - 3));
                    courbe.Liste_circle.Add(circle);
                }


            }
            courbe.color_curve(courbe.Colorname);


        }

        private void point_ajouter(object sender, RoutedEventArgs e)
        {
            if (trouvAB == true)
            {
                ajouter = true;
            }
            else
            {
                ajouter = false;

                MessageBox.Show("Vous ne pouvez pas ajouter un point au segment AB ", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void deplacer_click(object sender, RoutedEventArgs e)
        {
            if (trouvAB == true)
            {
                mouved = true;
            }
            else { mouved = false; MessageBox.Show("Le segment ne peut pas etre deplace ", "Alert", MessageBoxButton.OK, MessageBoxImage.Information); }
        }


        private void intersection(object sender, RoutedEventArgs e)

        {
            Intersection inter = new Intersection();


            intersection_points.Clear();
            intersection_ellipses.Clear();
            altitudes.Clear();
            coord.Clear();
            if ((SegmentAB.SegmentList.Count != 0) && (curvelist.Count != 0))
            {
                Point pty = new Point(0, 0);
                foreach (Curve cur in curvelist)
                {

                    foreach (Segment segg in cur.SegmentList)
                    {
                        inter.Intersection_segm(SegmentAB.SegmentList[0], segg);

                        if ((inter.trouv == true) && (pty != inter.Intersection_segm(SegmentAB.SegmentList[0], segg)))
                        {
                            intersection_points.Add(pty, cur.Altitude);
                            altitudes.Add(cur.Altitude);
                            coord.Add(pty);

                        }
                        else
                        {
                            if (pty != inter.Intersection_segm(SegmentAB.SegmentList[0], segg))
                            {
                                if (intersection_points.ContainsKey(inter.Intersection_segm(SegmentAB.SegmentList[0], segg))) { }

                                else
                                {
                                    intersection_points.Add(inter.Intersection_segm(SegmentAB.SegmentList[0], segg), cur.Altitude);
                                    altitudes.Add(cur.Altitude);
                                    coord.Add(inter.Intersection_segm(SegmentAB.SegmentList[0], segg));
                                }
                            }



                        }




                    }
                }
                foreach (KeyValuePair<Point, int> point in intersection_points)

                {
                    Ellipse Point_inter = new Ellipse()
                    {
                        Width = 6,
                        Height = 6,
                        Stroke = Brushes.Red,
                        StrokeThickness = 6
                    };
                    Point_inter.SetValue(Canvas.LeftProperty, (point.Key.X - 3));
                    Point_inter.SetValue(Canvas.TopProperty, (point.Key.Y - 3));
                    intersection_ellipses.Add(Point_inter);
                    canvas.Children.Add(Point_inter);


                }
            }

        }




    }
    [Serializable()]
    public class SaveFormat1 : ISerializable
    {// the class to be serialized by wpf
        public List<List<Point>> segments = new List<List<Point>>();
        public List<List<int>> altitude = new List<List<int>>();
        public List<Point> segmentAB = new List<Point>();
        public List<List<String>> couleurs = new List<List<String>>();
        public int MAX;
        public int MIN;
        public int Equidistance;
        public double Echelle;
        public String EchelleY;
        public String METRE;
        public String image;
        public int Color;
        public SaveFormat1()
        { }
        public SaveFormat1(String image, List<List<Point>> segments, List<List<int>> altitude, List<Point> segmentAB, List<List<String>> couleurs, int MAX, int MIN, int Equidistance, double Echelle, String EchelleY, String METRE, int Color)
        {
            this.segments = segments;
            this.altitude = altitude;
            this.couleurs = couleurs;
            this.image = image;
            this.segmentAB = segmentAB;
            this.MAX = MAX;
            this.MIN = MIN;
            this.Equidistance = Equidistance;
            this.Echelle = Echelle;
            this.EchelleY = EchelleY;
            this.METRE = METRE;
            this.Color = Color;

        }
        public SaveFormat1(SerializationInfo info, StreamingContext context)
        {
            segments = (List<List<Point>>)info.GetValue("segments:", typeof(List<List<Point>>));
            altitude = (List<List<int>>)info.GetValue("altitude", typeof(List<List<int>>));
            couleurs = (List<List<String>>)info.GetValue("couleurs", typeof(List<List<String>>));
            image = (String)info.GetValue("image", typeof(String));
            segmentAB = (List<Point>)info.GetValue("segmentAB:", typeof(List<Point>));
            MAX = (int)info.GetValue("max", typeof(int));
            MIN = (int)info.GetValue("min", typeof(int));
            Equidistance = (int)info.GetValue("Equidistance", typeof(int));
            Echelle = (double)info.GetValue("Echelle_x", typeof(double));
            EchelleY = (String)info.GetValue("Echelle_y", typeof(String));
            METRE = (String)info.GetValue("unité", typeof(String));
            Color = (int)info.GetValue("Color", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("altitude", altitude);
            info.AddValue("segment:", segments);
            info.AddValue("image:", image);
            info.AddValue("segmentAB:", segmentAB);
            info.AddValue("MAX", MAX);
            info.AddValue("MIN", MIN);
            info.AddValue(" Equidistance", Equidistance);
            info.AddValue("Echelle", Echelle);
            info.AddValue("EchelleY", EchelleY);
            info.AddValue("unitealt", METRE);
            info.AddValue("Color", Color);
        }
    }


}
