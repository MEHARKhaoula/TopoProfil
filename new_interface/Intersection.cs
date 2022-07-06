using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace new_interface
{
    class Intersection
    {
        public bool trouv;

        public bool Trouv { get => trouv; set => trouv = value; }

        public Point 
            Intersection_segm(Segment seg1, Segment seg2)
        {

            trouv = false;
            Point inter = new Point(1, 1);
            double x = 0;
            double y = 0;
            double A1 = seg1.b.Y - seg1.a.Y;
            double B1 = seg1.a.X - seg1.b.X;
            double C1 = A1 * seg1.a.X + B1 * seg1.a.Y;

            double A2 = seg2.b.Y - seg2.a.Y;
            double B2 = seg2.a.X - seg2.b.X;
            double C2 = A2 * seg2.a.X + B2 * seg2.a.Y;

            double numitor = A1 * B2 - A2 * B1;
            if (numitor == 0)
            {

                // inter = new Point(0, 0);
                trouv = true;


            }
            else
            {
                x = (B2 * C1 - B1 * C2) / numitor;
                y = (A1 * C2 - A2 * C1) / numitor;

                inter = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                if (inter == new Point(0, 0))//(x==0)&&(y==0)
                {

                    trouv = true;
                }
            }
            if (seg1.a.X < seg1.b.X)
            {
                if ((seg1.a.X < x) && (x < seg1.b.X))
                {

                }
                else
                {

                    return new Point();
                }
            }
            else
            {
                if ((seg1.a.X > x) && (x > seg1.b.X))
                {

                }
                else
                {

                    return new Point();
                }
            }
            if (seg1.a.Y < seg1.b.Y)
            {
                if ((seg1.a.Y < y) && (y < seg1.b.Y))
                {

                }
                else
                {

                    return new Point();
                }
            }
            else
            {
                if ((seg1.a.Y > y) && (y > seg1.b.Y))
                {

                }
                else
                {

                    return new Point();
                }
            }
            if (seg2.a.X < seg2.b.X)
            {
                if ((seg2.a.X < x) && (x < seg2.b.X))
                {

                }
                else
                {

                    return new Point();
                }
            }
            else
            {
                if ((seg2.a.X > x) && (x > seg2.b.X))
                {

                }
                else
                {

                    return new Point();
                }
            }
            if (seg2.a.Y < seg2.b.Y)
            {
                if ((seg2.a.Y < y) && (y < seg2.b.Y))
                {

                }
                else
                {

                    return new Point();
                }
            }
            else
            {
                if ((seg2.a.Y > y) && (y > seg2.b.Y))
                {

                }
                else
                {

                    return new Point();
                }
            }


            return inter;
        }
    }
}
