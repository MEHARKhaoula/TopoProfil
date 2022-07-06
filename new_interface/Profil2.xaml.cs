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
    /// Logique d'interaction pour Profil2.xaml
    /// </summary>
    public partial class Profil2 : Window
    {
        private Pente pente; // la classe qui contient les méthodes de calcule de la pente
        private Label DataLabel = null;
        private double scalex; // chaque unité en réalité sur l'axe des x ce qui lui correspond en pixel
        private double scaley; // chaque unité en réalité sur l'axe des y ce qui lui correspond en pixel
        private double wxmin; // egale à 0
        private double wxmax; // la plus grande valeur sur l'axe des x
        private double wymin; // la plus petite valeur sur l'axe des y
        private double wymax; // la plus grande valeur sur l'axe des y
        private double dxmin = 80; // l'abscice  de l'origine par rapport au canvas
        public double dxmax; // la longueur du segment AB qui sera égale à la longueur de l'axe x
        private double dymin = margin; // l'ordonne  de l'origine par rapport au canvas
        public double dymax;  // la longueur de l'axe y en pixel
        private string united; // unite de l'axe des distances
        private string uniteal; // unite de l'axe des altitudes
        const double margin = 40;  // la marge du graphe par rapport canvas

        public List<point> liste_cordonnes; // la liste des coordonnes
        
        private List<shape> liste_circles; // la liste des cercles qui constituent les cordoonnées du graphe
        public List<Path> liste_segment; // la liste des segments qui constituent le graphe
        public FrameworkElement selectedPath, selectedEllipse; // le segment selectionné pour afficher la pente
       private double equidistance; 
     
        public Color color;  // la couleur du graphe
        public Color back; // le background du profil
        pte po;



        public Profil2(double wxmax, double wymin, double wymax, double dxmax, double equidistance, List<point> liste_cordonnes, string united, string uniteal, Color color, Color back)
        {/** initialise les attributs de essentiels pour tracer le graphe**/
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.wxmin = 0;
            this.wxmax = wxmax;
            this.wymin = wymin;
            this.wymax = wymax;
            this.liste_cordonnes = liste_cordonnes;
            this.dxmax = dxmax;
            this.equidistance = equidistance;
            this.united = united;
            this.uniteal = uniteal;
            this.color = color;
            this.back = back;
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {/**Fermer la fenetre du profil**/
            this.Close();
        }
        private void ButtonMaximise_Click(object sender, RoutedEventArgs e)
        {/**Maximiser la fenetre du profil**/
            if (this.WindowState == WindowState.Normal)
            {
                GridMain.Width = 1365;

                //ImgOriginal.Height = 800;
                this.WindowState = WindowState.Maximized;
                GridMain.Height = 800;
                src.Width = 1365;
                src.Height = 800;

                //  canvas.Width = 400;
                //canvas.Height = 800;
                canGraph.Width = 700;
                canGraph.Height = 400;
                scvGraph.Height = 800;



            }
            else if (this.WindowState == WindowState.Maximized)
            {
                // this.Width = SystemParameters.WorkArea.Width;
                //this.Height = SystemParameters.WorkArea.Height;
                this.WindowState = WindowState.Normal;
                GridMain.Width = 800;
                GridMain.Height = 500;
                src.Width = 800;
                canGraph.Width = 576;
                canGraph.Height = 240;
                scvGraph.Height = 395;
                // canvas.Width = 211;
                //canvas.Height = 536;


            }

        }

        private void ButtonImprimer_Click(object sender, RoutedEventArgs e)
        { /**Imprimer le graphe cette méthode est liée à l'icone imprimer**/

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {

                printDialog.PrintVisual(scvGraph, "Printing");
            }

        }
        private void ButtonMinimise_Click(object sender, RoutedEventArgs e)
        {/**Minimiser la fenetre cette méthode est liée à l'icone minimiser **/
            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        public List<point> Liste_cordonnes { get => liste_cordonnes; set => liste_cordonnes = value; } 

       



        public void Window_Loaded(object sender, RoutedEventArgs e) 
        {
           /**Apres appel du constructeur de la classe cette méthode dessine le rectangle dans lequel on va dessiner le graphe et fait appel au méthodes qui se chargent de dessiner le graphe**/
            canGraph.Width = dxmax + margin * 3; // la largeur du canvas canGraph
            dxmax = dxmax + 80;       

            dymax = canGraph.Height - margin; // la longueur de l'axe y 
            Rectangle rec = new Rectangle();
            Canvas.SetLeft(rec, 2 * margin + 1);
            Canvas.SetTop(rec, margin - 1);
            rec.Fill = new SolidColorBrush(Colors.Lavender); // initialisé avec la couleur lavendar
            rec.Width = dxmax - 2 * margin;
            rec.Height = canGraph.Height - 2 * margin;
            canGraph.Children.Add(rec);
            Definescale(); 
            draw_axis();
            draw_profile();

        }



        public void Definescale()
        { 
            scalex = (dxmax - dxmin) / (wxmax - wxmin); //chaque unité en réalité ce qui lui correspond en pixel sur l'axe x
            scaley = (dymax - dymin) / (wymax - wymin); //chaque unité en réalité ce qui lui correspond en pixel sur l'axe y

        }



     



        public System.Windows.Point from_world_to_device(System.Windows.Point point)
        {/**Cette fonction permet de convertir  un point à paritir des coordonnées réelle en coordonnées pixels en utilisant scalex et scaley* */
            double x = point.X;
            double y = point.Y;
            double newx = (x - wxmin) * scalex + dxmin;
            double newy = (y - wymin) * scaley + dymin;
            newy = dymax - newy + dymin;
            return new System.Windows.Point(newx, newy);
        }



        public void draw_axis()
        {/**Cette méthode permet de dessiner les axes x et y **/

            Rectangle rec = new Rectangle(); // redessigner le rectangle (le cas ou il s'agit d'une modification//
            Canvas.SetLeft(rec, 2 * margin + 1);
            Canvas.SetTop(rec, margin - 1);
            rec.Fill = new SolidColorBrush(back);
            rec.Width = dxmax - 2 * margin;
            rec.Height = canGraph.Height - 2 * margin;
            canGraph.Children.Add(rec);

            double val = wxmax / 5; // on dévise la plus grande valeur sur l'axe des x par 5
            int s = Convert.ToInt32(val); // on transforme le résultat en un entier
            while ((s % 5) != 0)
            {
                s++;   // on incrémente le résultat jusqu'à obtenir un nombre multiple de 5 pour graduer les axes avec des multiples de 5
            }
            GeometryGroup xaxis_geom = new GeometryGroup();   
            xaxis_geom.Children.Add(new LineGeometry(
                new System.Windows.Point(2 * margin, dymax), new System.Windows.Point(dxmax, dymax))); // dessiner la ligne de l'axe x
            double cpt = 1;
            double h = s;
            while (h <= wxmax) // cette boucle sert à graduer les axe x
            {

                // h c'est l'abssice du petit segment de la graduation de l'axe en réellité
                double newx = (h - wxmin) * scalex + dxmin; // newx c'est l'abssice du petit segment de la graduation de l'axe en pixel
                                                            // son ordonnée c'est dymax-margin / 10 et dymax + margin / 10

                xaxis_geom.Children.Add(new LineGeometry( 
                  new System.Windows.Point(newx, dymax - margin / 10),
                  new System.Windows.Point(newx, dymax + margin / 10)));
                DrawText(canGraph, h.ToString(), new System.Windows.Point(newx, canGraph.Height - margin / 2), 11, HorizontalAlignment.Center, VerticalAlignment.Center, 0, false); // afficher h à coté de la graduation
                cpt++;
                h = s * cpt; // pour passer à la deuxième graduation

            }

            xaxis_geom.Children.Add(new LineGeometry( // écrire "0" à l'origine du repère
                  new System.Windows.Point(2 * margin, dymax - margin / 10),
                  new System.Windows.Point(2 * margin, dymax + margin / 10)));
            DrawText(canGraph, "0", new System.Windows.Point(2 * margin, canGraph.Height - margin / 2), 11, HorizontalAlignment.Center, VerticalAlignment.Center, 0, false);
            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;
            canGraph.Children.Add(xaxis_path);

            double val2 = (wymax - wymin) / 5; // on refait le meme traitement avec l'axe y
            int s2 = Convert.ToInt32(val2);
            while ((s2 % 5) != 0)
            {
                s2++;
            }
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new System.Windows.Point(dxmin, margin), new System.Windows.Point(dxmin, canGraph.Height - margin)));
            cpt = 1;

            double h2 = wymin + s2;

            do
            {
                // cette boucle sert àgraduer l'axe y et  dessiner des lignes pointillées pour graduation de l'axe y

                double newy = (h2 - wymin) * scaley + dymin;
                newy = dymax - newy + dymin; // newy c'est l'ordonnée de la ligne pointillé en pixel et la la graduation


                yaxis_geom.Children.Add(new LineGeometry( // le dessin de la graduation
                  new System.Windows.Point(dxmin - margin / 10, newy),
                  new System.Windows.Point(dxmin + margin / 10, newy)));
                DrawText(canGraph, h2.ToString(), new System.Windows.Point(dxmin - margin / 2, newy), 11, HorizontalAlignment.Center, VerticalAlignment.Center, 0, false);
                h2 = h2 + s2;
                // le dessin de la ligne pointillé
                LineGeometry line = new LineGeometry(new System.Windows.Point(dxmin, newy), new System.Windows.Point(dxmax, newy));
                Path dashed_line = new Path();
                dashed_line.StrokeDashArray = new DoubleCollection() { 8, 2 };
                dashed_line.StrokeThickness = 1;
                dashed_line.Data = line;
                dashed_line.Stroke = Brushes.Gray;
                canGraph.Children.Add(dashed_line);

            } while (h2 <= wymax);
            //afficher le wymin 
            DrawText(canGraph, wymin.ToString(), new System.Windows.Point(dxmin - margin / 2, dymax), 11, HorizontalAlignment.Center, VerticalAlignment.Center, 0, false);
            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;
            canGraph.Children.Add(yaxis_path);



        }


        public void draw_profile()

        {/**Cette méthode permet d'initialiser les coordonnées et dessiner les segments du graphe**/
            liste_segment = new List<Path>(200);
            liste_circles = new List<shape>(200);
            intialise_cordinates();




            for (int i = 0; i < pente.tab_pente.Count; i++)
            {
                Point p1 = new Point(pente.tab_pente[i].get_x1(), pente.tab_pente[i].get_y1()); 
                Point p2 = new Point(pente.tab_pente[i].get_x2(), pente.tab_pente[i].get_y2());
                p1 = from_world_to_device(p1); // convertir au coordonnées du canvas
                p2 = from_world_to_device(p2);
                GeometryGroup segment = new GeometryGroup();
                segment.Children.Add(new LineGeometry(p1, p2)); // dessiner un segment qui traversent ses deux points
                liste_segment.Add(new Path());
                liste_segment[i].StrokeThickness = 2;


                liste_segment[i].Stroke = new SolidColorBrush(color);
                liste_segment[i].Data = segment;
                canGraph.Children.Add(liste_segment[i]);
            }

            for (int i = 0; i < Liste_cordonnes.Count; i++)
            { /**dessiner les cercles qui représentent les coordonnées **/
                Point p = new Point(Liste_cordonnes[i].get_x(), Liste_cordonnes[i].get_y());
                Point p1 = from_world_to_device(p);
                liste_circles.Add(new shape(p1.X - 2, p1.Y - 2, 4, 4, canGraph, color));
                liste_circles[i].world_point = p; // le point qui sera affiché quand on clique sur cette cercle
            }
            // ecrire les unites des distances et altitudes
            DrawText(canGraph, "La distance en " + united, new Point((dxmax + 40) / 2, dymax + 40),
     16,
     HorizontalAlignment.Center, VerticalAlignment.Center, 0, true);
            DrawText(canGraph, "Alititude en " + uniteal, new Point(30, dymax / 2),
    16,
    HorizontalAlignment.Center, VerticalAlignment.Center, 90, true);
        }


        public void intialise_cordinates()
        {
            int i = 0;
            int max = liste_cordonnes.Count;
            while (i < max)
            {  // cette boucle sert à régler les points bas et sommet du profile
                if ((i + 1) < max)
                {  
                    if (liste_cordonnes[i].get_y() == liste_cordonnes[i + 1].get_y()) // si deux altitudes successives ont la meme altitude

                    {
                        if (i > 0)
                            if (liste_cordonnes[i - 1].get_y() < liste_cordonnes[i].get_y())
                            {
                                // dans ce cas on ajoute un troisème point entre eux avec altitude qui sera égale à liste_cordonnes[i].get_y() + equidistance / 2 (point haut)
                                liste_cordonnes.Add(new point((liste_cordonnes[i].get_x() + liste_cordonnes[i + 1].get_x()) / 2, liste_cordonnes[i].get_y() + equidistance / 2));
                            }
                            else
                            {
                                // dans ce cas on ajoute un troisème point entre eux avec altitude qui sera égale à liste_cordonnes[i].get_y() - equidistance / 2 (point bas)
                                liste_cordonnes.Add(new point((liste_cordonnes[i].get_x() + liste_cordonnes[i + 1].get_x()) / 2, liste_cordonnes[i].get_y() - equidistance / 2));
                            }
                        else
                        {
                            if (i < Liste_cordonnes.Count - 2)
                            {
                                if (liste_cordonnes[i + 1].get_y() < liste_cordonnes[i + 2].get_y())
                                {
                                    // dans ce cas on ajoute un troisème point entre eux avec altitude qui sera égale à liste_cordonnes[i].get_y() + equidistance / 2 (point haut)
                                    liste_cordonnes.Add(new point((liste_cordonnes[i].get_x() + liste_cordonnes[i + 1].get_x()) / 2, liste_cordonnes[i].get_y() - equidistance / 2));
                                }
                                else
                                {    // dans ce cas on ajoute un troisème point entre eux avec altitude qui sera égale à liste_cordonnes[i].get_y() - equidistance / 2 (point bas)
                                    liste_cordonnes.Add(new point((liste_cordonnes[i].get_x() + liste_cordonnes[i + 1].get_x()) / 2, liste_cordonnes[i].get_y() + equidistance / 2));
                                }
                            }
                        }
                    }
                }
                i++;
            }
            int K, I, J;
            for (K = 0; K < liste_cordonnes.Count; K++)
            {
                // on utilisé le tri par bulle pour trier la liste des coordonnées selon l'abscisse
                for (I = liste_cordonnes.Count - 2; I >= 0; I--)
                {
                    for (J = 0; J <= I; J++)
                    {
                        if (liste_cordonnes[J + 1].get_x() < liste_cordonnes[J].get_x())
                        {
                            point t = liste_cordonnes[J + 1];
                            liste_cordonnes[J + 1] = liste_cordonnes[J];
                            liste_cordonnes[J] = t;

                        }
                    }
                }
            }
            pente = new Pente(liste_cordonnes);
            // initialiser le tableau qui contient les pentes
            pente.Calculer_pente();





        }

       
        ///write a text 
        private void DrawText(Canvas can, string text, System.Windows.Point location,
     double font_size,
     HorizontalAlignment halign, VerticalAlignment valign, double angle, bool c)
        {
            //  Construire  label.

            Label label = new Label();
            label.Content = text;
            label.FontSize = font_size;
            can.Children.Add(label);
            if (angle != 0)
                label.LayoutTransform = new RotateTransform(angle);
            label.FontWeight = FontWeights.Bold;
            label.Foreground = Brushes.Black;
            if (c == true)
            {
                label.FontFamily = new FontFamily("Times New Roman");
                label.FontStyle = FontStyles.Italic;
            }
            // Positionner the label.
            label.Measure(new Size(double.MaxValue, double.MaxValue));

            double x = location.X;
            if (halign == HorizontalAlignment.Center)
                x -= label.DesiredSize.Width / 2;
            else if (halign == HorizontalAlignment.Right)
                x -= label.DesiredSize.Width;
            Canvas.SetLeft(label, x);

            double y = location.Y;
            if (valign == VerticalAlignment.Center)
                y -= label.DesiredSize.Height / 2;
            else if (valign == VerticalAlignment.Bottom)
                y -= label.DesiredSize.Height;
            Canvas.SetTop(label, y);
        }

        double pi;
        private void MouseDownClick(object sender, MouseButtonEventArgs e)
        { /** Cette fonction permet de trouver le segment ou le cercle sélectionné dans le graphe**/
            object TestPanelOrUI = canGraph.InputHitTest(e.GetPosition(canGraph)) as FrameworkElement;
            if (TestPanelOrUI is Path || TestPanelOrUI is Ellipse)
            { //po est la fentre qui affiche la pente
                if (TestPanelOrUI is Path) // sil s'agit d'un segment
                {

                    Path path = (Path)TestPanelOrUI;
                    foreach (Path p in liste_segment) 
                    {
                        if (p.Equals(path))
                        {
                            if (po != null)

                            {
                                po.Close();
                            }
                            // on le colore avec une couleur plus claire
                            p.Stroke = Brushes.DarkTurquoise;





                        }
                    }
                    if (selectedPath == null)
                    {
                        int K = 0;
                        selectedPath = (FrameworkElement)TestPanelOrUI;
                        foreach (Path p in liste_segment)
                        {
                            if (p.Equals(((Path)selectedPath)))
                            {
                                p.Stroke = Brushes.DarkTurquoise;
                                pi = pente.tab_pente[K].get_pente(); // on accede à la pente de sce segment
                                pi = Math.Round(pi, 3);

                                po = new pte(pi);
                                po.ShowActivated = true; // afficher dans la fentre
                                po.ShowDialog();
                            }
                            K++;
                        }
                    }
                    else
                    {
                        if (selectedPath.Equals((FrameworkElement)TestPanelOrUI) == false) // si le segment sélectionné n'est pas égale au segment sélectionné maintenant
                        { 


                            foreach (Path p in liste_segment)
                            { // on le recolore avec sa couleur réelle
                                if (p.Equals(((Path)selectedPath)))
                                {   
                                    p.Stroke = new SolidColorBrush(color);
                                    po.Close();

                                }


                            }

                            selectedPath = (FrameworkElement)TestPanelOrUI;
                            int K = 0;
                            foreach (Path p in liste_segment)
                            { // colorer le nouveau segment
                                if (p.Equals(((Path)selectedPath)))
                                {

                                    pi = pente.tab_pente[K].get_pente();
                                    pi = Math.Round(pi, 3);
                                   
                                    po = new pte(pi);
                                    po.ShowActivated = true;
                                    po.ShowDialog();

                                }
                                K++;
                            }


                        }
                    }
                }
                if (TestPanelOrUI is Ellipse) // s'il s'agit d'un cercle
                {

                    if ((selectedEllipse == null) || (selectedEllipse.Equals((FrameworkElement)TestPanelOrUI) == true))
                    {
                        selectedEllipse = (FrameworkElement)TestPanelOrUI;
                        // construire le label qui vas afficher les coordonnées
                        DataLabel = new Label(); 
                        DataLabel.Foreground = Brushes.Black;
                        DataLabel.FontWeight = FontWeights.Bold;
                        foreach (shape ell in liste_circles)
                        {
                            if (ell.circle.Equals((Ellipse)TestPanelOrUI))
                            {
                                // afficher trois chiffres apres la virgule
                                System.Windows.Point w = ell.world_point;
                                w = new Point(Math.Round(w.X, 3), Math.Round(w.Y, 3));
                                DataLabel.Content = "(" +
                         w.X.ToString() + "     ,   " +
                          w.Y.ToString() + ")";
                            }
                        }

                        DataLabel.FontSize = 12;


                        canGraph.Children.Add(DataLabel);
                        
                        DataLabel.Measure(new Size(double.MaxValue, double.MaxValue));

                        double x = e.GetPosition(canGraph).X;

                        x -= DataLabel.DesiredSize.Width / 2;
                        Canvas.SetLeft(DataLabel, x);

                        double y = e.GetPosition(canGraph).Y;

                        y -= DataLabel.DesiredSize.Height / 2;

                        Canvas.SetTop(DataLabel, y);
                    }
                    else if (selectedEllipse.Equals((FrameworkElement)TestPanelOrUI) == false)
                    {
                        canGraph.Children.Remove(DataLabel); // supprimer l'ancien label
                        //constrire un nouveau label
                        DataLabel = new Label();
                        DataLabel.FontWeight = FontWeights.Bold;
                        foreach (shape ell in liste_circles)
                        {
                            if (ell.circle.Equals((Ellipse)TestPanelOrUI))
                            {

                                System.Windows.Point w = ell.world_point;
                                w = new Point(Math.Round(w.X, 3), Math.Round(w.Y, 3));
                                DataLabel.Content = "(" +
                         w.X.ToString() + "     ,   " +
                          w.Y.ToString() + ")";
                            }
                        }
                        DataLabel.FontSize = 12;
                        DataLabel.FontWeight = FontWeights.Bold;
                        DataLabel.Foreground = Brushes.Black;

                        canGraph.Children.Add(DataLabel);
                       
                        DataLabel.Measure(new Size(double.MaxValue, double.MaxValue));

                        double x = e.GetPosition(canGraph).X;

                        x -= DataLabel.DesiredSize.Width / 2;
                        Canvas.SetLeft(DataLabel, x);

                        double y = e.GetPosition(canGraph).Y;

                        y -= DataLabel.DesiredSize.Height / 2;

                        Canvas.SetTop(DataLabel, y);


                        selectedEllipse = (FrameworkElement)TestPanelOrUI;
                    }
                }
            }

            else
            {
                // s'il n'a pas choisit un cercle ou un segment
                if (selectedPath != null)
                {
                    foreach (Path p in liste_segment)
                    {
                        if (p.Equals(((Path)selectedPath)))
                        {
                            p.Stroke = new SolidColorBrush(color);
                        }
                    }
                    selectedPath = null;
                }
                if (DataLabel != null)
                {
                    canGraph.Children.Remove(DataLabel);
                }
            }
        }

        private void zoomer(object sender, RoutedEventArgs e)
        { /**afficher la fenetre du zoom **/
            Zoom z = new Zoom();
            z.profil = this;
            z.Show();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        { /**la fenetre de la modification **/
            Window1 p = new Window1();
            p.w1 = this;
            p.Show();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Modifier_Loaded()
        {
            /**changer les styles du graphe selon les parametres choisie dans la fenetre Window1 **/
            canGraph.Width = dxmax + margin * 3;
            dxmax = dxmax + 80;

            dymax = canGraph.Height - margin;

            Definescale();
            draw_axis();
            draw_profile();

        }
    }
}
