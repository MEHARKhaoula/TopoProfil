using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
namespace new_interface
{
    class shape
    {
        public Ellipse circle; // l'élent graphiqique
        public System.Windows.Point world_point; //le point correspondant

        public shape(double x, double y, int width, int height, Canvas cv , Color cl) // le constructeur
        {

            circle = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = new SolidColorBrush(cl),
                StrokeThickness = 4
            };

            cv.Children.Add(circle);

            circle.SetValue(Canvas.LeftProperty, x);
            circle.SetValue(Canvas.TopProperty, y);

        }

        public Action<MouseButtonEventArgs> MouseLeftButtonDown { get; internal set; }
    }
}



