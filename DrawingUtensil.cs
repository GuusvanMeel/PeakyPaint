using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PeakyPaint
{
    
    class DrawingUtensil
    {
        public Brush Brush { get; set; }
        
        public double Thickness { get; set; }
        private Canvas canvas;

      public DrawingUtensil(Canvas _canvas)
        {
            canvas = _canvas;
            Brush = new SolidColorBrush();
        }
        public Polyline Line(Brush brush, int thickness)
        {
            Polyline line = new Polyline
            {
                Stroke = brush,
                StrokeThickness = thickness
            };


            return line;
        }
       
    }
}
