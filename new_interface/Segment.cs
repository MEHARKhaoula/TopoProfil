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
    class Segment
    {
        public Point a;
        public Point b;
        private Polyline line;

        //getters and setters

        public Point A { get => this.a; set => this.a = value; }
        public Polyline Line { get => line; set => line = value; }
        public Point B { get => b; set => b = value; }

        //constructeur segment 
        public Segment()
        {
            this.line = new Polyline
            {

                Stroke = SystemColors.WindowFrameBrush,
                StrokeThickness = 3.0

            };

        }

        //dessiner segment
        public void Draw_segment(Canvas canvas)
        {

            line.Points.Add(a);
            line.Points.Add(b);
            canvas.Children.Add(line);

        }
        public void color_segment(String color)
        {
            line.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }
    }
}
