using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PeakyPaint
{
    
    class DrawingUtensil
    {
        public Brush? Brush { get; set; }
        
        public double Thickness { get; set; }

        // Constructor for SolidColorBrush or other Brush types
        public DrawingUtensil(Brush brush, double thickness)
        {
            this.Brush = brush;
            this.Thickness = thickness;
        }

        // Constructor for LinearGradientBrush
        public DrawingUtensil(LinearGradientBrush gradient, double thickness)
        {
           
            this.Thickness = thickness;
            this.Brush = gradient; // This ensures Brush is also set, as LinearGradientBrush is a subclass of 
        }

        // Constructor for RadialGradientBrush
        public DrawingUtensil(RadialGradientBrush gradient, double thickness)
        {
            
            this.Thickness = thickness;
            this.Brush = gradient; // This ensures Brush is also set, as RadialGradientBrush is a subclass of Brush
        }

        // Constructor for ImageBrush
        public DrawingUtensil(ImageBrush imageBrush, double thickness)
        {
            this.Brush = imageBrush; // ImageBrush inherits from Brush
            this.Thickness = thickness;
        }

        // Constructor for DrawingBrush
        public DrawingUtensil(DrawingBrush drawingBrush, double thickness)
        {
            this.Brush = drawingBrush; // DrawingBrush inherits from Brush
            this.Thickness = thickness;
        }

        // Constructor for VisualBrush
        public DrawingUtensil(VisualBrush visualBrush, double thickness)
        {
            this.Brush = visualBrush; // VisualBrush inherits from Brush
            this.Thickness = thickness;
        }
     
    }
}
