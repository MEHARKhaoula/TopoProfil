using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_interface
{
    public class Coordonne
    {
        double x1, y1, x2, y2, pente;
        public Coordonne(double x1, double x2, double pente)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.pente = pente;

        }
        public Coordonne(double x1, double x2, double y1, double y2, double pente)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.pente = pente;

        }
        public void set_x1_y1(double x1, double y1)
        {
            this.x1 = x1;
            this.y1 = y1;
        }
        public void set_x2_y2(double x2, double y2)
        {
            this.x2 = x2;
            this.y2 = y2;
        }
        public void set_pente(double pente)
        {
            this.pente = pente;
        }
        public double get_x1() { return this.x1; }
        public double get_y1() { return this.y1; }
        public double get_x2() { return this.x2; }
        public double get_y2() { return this.y2; }
        public double get_pente() { return this.pente; }
    }

}
