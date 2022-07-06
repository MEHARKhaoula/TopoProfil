using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_interface
{
    public class point
    {
        double x, y;
        public point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public void set_x_y(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double get_x() { return this.x; }
        public double get_y() { return this.y; }

    }
}
