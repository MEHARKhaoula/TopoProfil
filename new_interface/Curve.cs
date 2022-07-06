using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace new_interface
{
    class Curve
    {
        private int altitude;
        static private int numordre;
        private String colorname;
        private Segment seg;
        private int cpt;
        private Point pointdebut;
        private Point pointcourant;
        bool newline = false;
        private int get_numordre;
        double lbx;
        double lby;
        double lax;
        double lay;
        Point a = new Point();
        Point b = new Point();
        public Point Pointdebut { get => pointdebut; set => pointdebut = value; }
        public List<Ellipse> Liste_circle { get => liste_circle; set => liste_circle = value; }
        public List<Segment> SegmentList { get => segmentlist; set => segmentlist = value; }

        private List<Segment> segmentlist = new List<Segment>();

        private List<Ellipse> liste_circle = new List<Ellipse>();
        private List<Ellipse> liste_circle2 = new List<Ellipse>();
        public List<Ellipse> Liste_circle2 { get => liste_circle2; set => liste_circle2 = value; }


        public int Altitude { get => altitude; set => altitude = value; }
        public string Colorname { get => colorname; set => colorname = value; }
        public int Cpt { get => cpt; set => cpt = value; }
        public Segment Seg { get => seg; set => seg = value; }
        public int Get_numordre { get => get_numordre; set => get_numordre = value; }

        public List<Segment> romaissa = new List<Segment>();


        public void inc_numordre() { numordre++; }




        public List<Ellipse> Draw_curve_leftclick(Point position, Canvas canvas)
        {

            if (Cpt > 1)
            {
                segmentlist.Add(seg);
            }
            //circle(position.X - 3, position.Y - 3, 6, 6, canvas);
            Ellipse circle = new Ellipse()
            {
                Width = 6,
                Height = 6,
                Stroke = Brushes.Red,
                StrokeThickness = 6
            };

            canvas.Children.Add(circle);
            circle.SetValue(Canvas.LeftProperty, (position.X - 3));
            circle.SetValue(Canvas.TopProperty, (position.Y - 3));
            liste_circle.Add(circle);

            newline = true;
            Cpt = 1;
            pointdebut = position;
            seg = new Segment();
            seg.A = pointdebut;

            return liste_circle;
        }


        public void Draw_curve_mousemove(Point position, Canvas canvas)
        {
            if (newline)
            {
                if (Cpt > 1)
                {
                    seg.Line.Points.Remove(pointcourant);
                    canvas.Children.Remove(this.seg.Line);
                }

                pointcourant = position;
                seg.B = pointcourant;
                seg.Draw_segment(canvas);
                Cpt++;
            }
        }

        public static void circle(Double x, Double y, int width, int height, Canvas cv)
        {

            Ellipse circle = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Red,
                StrokeThickness = 6
            };

            cv.Children.Add(circle);

            circle.SetValue(Canvas.LeftProperty, (x));
            circle.SetValue(Canvas.TopProperty, (y));
        }

        public void color_curve(String color)
        {
            foreach (Segment seg in segmentlist)
            {
                seg.color_segment(color);
            }

            this.colorname = color;
        }


        public Ellipse ajout_point_leftclick(Point position, Canvas canvas, Segment seg_mouve)
        {
            int dpt = 0;
            int dpt2 = 0;
            bool egal = false;
            Ellipse circle = new Ellipse()
            {
                Width = 6,
                Height = 6,
                Stroke = Brushes.Red,
                StrokeThickness = 6
            };

            canvas.Children.Add(circle);
            circle.SetValue(Canvas.LeftProperty, (position.X - 3));
            circle.SetValue(Canvas.TopProperty, (position.Y - 3));
            // liste_circle.Add(circle);
            Segment seg1 = new Segment();
            Segment seg2 = new Segment();
            seg1.A = seg_mouve.A;
            seg1.B = position;
            seg2.A = position;
            seg2.B = seg_mouve.B;
            foreach (Segment seg in segmentlist)
            {
                if (seg.Line.Equals(seg_mouve.Line))
                {

                    egal = true;
                    dpt = dpt2;

                }

                dpt2++;
            }
            segmentlist.Remove(seg_mouve);
            segmentlist.Insert(dpt, seg1);
            segmentlist.Insert(dpt + 1, seg2);
            liste_circle.Insert(dpt + 1, circle);
            // segmentlist.Add(seg1);
            // segmentlist.Add(seg2);
            // seg1.color_segment(colorname);

            // seg2.color_segment("White");
            canvas.Children.Remove(seg_mouve.Line);
            canvas.Children.Add(seg1.Line);
            canvas.Children.Add(seg2.Line);

            return circle;
        }
        public Segment mouve_curve_mousemove(Point position, Canvas canvas, Segment seg_mouve)
        {

            bool ch = false;
            double segx;
            double segy;

            segx = -seg_mouve.A.X + position.X;
            segy = -seg_mouve.A.Y + position.Y;
            romaissa.Clear();
            Liste_circle2.Clear();

            lbx = 0;
            lby = 0;
            lax = 0;
            lay = 0;
            a = new Point();
            b = new Point();

            /* if (lp ==0)
             { Segmentlistmouve= segmentlist; }*/
            /*  if (lp> 1)
          {

                foreach (Segment segmt in romaissa)
                  {


                  segmt.Line.Points.Remove(Pointdebut);
                      segmt.Line.Points.Remove(pointcourant);
                      canvas.Children.Remove(segmt.Line);


                  }
                  foreach (Ellipse Ell in Liste_circle2)
                  {

                      canvas.Children.Remove(Ell);


                  }
              }*/


            foreach (Segment segmm in segmentlist)
            {
                ch = false;
                if ((segmm.B.X == seg_mouve.B.X) && (segmm.B.Y == seg_mouve.B.Y) && (segmm.A.X == seg_mouve.A.X) && (segmm.A.Y == seg_mouve.A.Y))
                { ch = true; }
                lbx = segmm.B.X + segx;
                lby = segmm.B.Y + segy;

                lax = segmm.A.X + segx;
                lay = segmm.A.Y + segy;
                /* if (ch == true)
                   {
                       lax = position.X ;
                       lay = position.Y ;
                   }*/

                a = new Point(lax, lay);
                b = new Point(lbx, lby);

                Ellipse circle = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };


                circle.SetValue(Canvas.LeftProperty, (a.X - 3));
                circle.SetValue(Canvas.TopProperty, (a.Y - 3));

                if (Liste_circle2.Contains(circle))
                {

                }
                else
                {
                    Liste_circle2.Add(circle);
                }
                //__________
                Ellipse circl = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Red,
                    StrokeThickness = 6
                };


                circl.SetValue(Canvas.LeftProperty, (b.X - 3));
                circl.SetValue(Canvas.TopProperty, (b.Y - 3));

                if (Liste_circle2.Contains(circl))
                {

                }
                else
                {
                    Liste_circle2.Add(circl);
                }

                //_______________



                Segment sggm = new Segment();
                sggm.A = a;
                sggm.B = b;

                if (ch == true)
                { seg_mouve = sggm; }

                // Segmentlistmouve2.Add(sggm);
                sggm.Draw_segment(canvas);

                romaissa.Add(sggm);
                sggm.color_segment(colorname);
                segmm.Line.Points.Remove(Pointdebut);
                segmm.Line.Points.Remove(pointcourant);
                canvas.Children.Remove(segmm.Line);


            }





            //lp++;
            foreach (Ellipse Ell in liste_circle)
            {

                canvas.Children.Remove(Ell);
            }
            liste_circle.Clear();
            liste_circle.AddRange(Liste_circle2);


            segmentlist.Clear();
            segmentlist.AddRange(romaissa);
            return seg_mouve;
        }

    }
}
