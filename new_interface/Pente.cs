using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_interface
{



    public class Pente
    {
        public List<point> points = new List<point>();
        public List<Coordonne> tab_pente = new List<Coordonne>(200);
        public List<point> tab_pile = new List<point>(5);


        public void initialiser()
        {
            for (int i = 1; i < 200; i++)
            {
                tab_pente.Add(new Coordonne(0, 0, 0, 0, 0));
            }
        }
        public void initialiser_pile()
        {
            for (int i = 1; i < 200; i++)
            {
                tab_pile.Add(new point(0, 0));
            }
        }


        public Pente(List<point> lists)
        {
            points.AddRange(lists);
        }
        public void Calculer_pente()
        {
            point p, q;
            int cpt = 0;
            bool continuu = true;
            double pente1, pente2;
            q = points[1];
            int taille;
            while (points != null && continuu)
            {
                taille = points.Count;
                p = points.First();
               q = points[1];
                pente1 = (q.get_y() - p.get_y()) / (q.get_x() - p.get_x());
                tab_pile.Add(p);
                tab_pile.Add(q);
                points.RemoveAt(0);
                p = points.First();
                if (points.Count == 1)
                {
                    q = p;
                    p = tab_pile[1];
                    pente2 = (q.get_y() - p.get_y()) / (q.get_x() - p.get_x());
                    if (pente1 != pente2)
                    {
                        Calculer(cpt);

                        cpt++;
                        tab_pile.Clear();
                    }
                    else
                    {
                        points.RemoveAt(0);
                        calculer_pente_egaux(pente1, cpt);
                        cpt++;
                    }

                }
                if (points.Count >= 2)
                {
                    q = points[1];
                    pente2 = (q.get_y() - p.get_y()) / (q.get_x() - p.get_x());
                    if (pente1 != pente2)
                    {
                        Calculer(cpt);

                        cpt++;
                        tab_pile.Clear();
                    }
                    else
                    {
                        points.RemoveAt(0);
                        calculer_pente_egaux(pente1, cpt);
                        cpt++;
                    }

                }


                else
                {
                    continuu = false;

                }
            }
        }
        public void Calculer(int cpt)
        {
            double pent = (100 * (tab_pile[1].get_y() - tab_pile[0].get_y())) / (tab_pile[1].get_x() - tab_pile[0].get_x());
            tab_pente.Add(new Coordonne(tab_pile[0].get_x(), tab_pile[1].get_x(), tab_pile[0].get_y(), tab_pile[1].get_y(), pent));
        }
        public void calculer_pente_egaux(double pente, int cpt)
        {
            double pente_i;
            bool continu = true;
            point p, q;
            tab_pile.RemoveAt(1);

            while (continu)
            {
                p = points.First();
                q = points[1];
                pente_i = ((q.get_y() - p.get_y()) / (q.get_x() - p.get_x()));
                if (pente == pente_i)
                {
                    points.RemoveAt(0);
                }
                else
                {
                    tab_pile.Add(p);
                    continu = false;
                    Calculer(cpt);
                    tab_pile.Clear();
                }
            }
        }
    }
}
    


