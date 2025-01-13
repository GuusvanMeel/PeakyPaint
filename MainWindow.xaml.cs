using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private bool isDrawing = false; // Flag to track drawing state
        private Polyline currentLine; // The current line being drawn
        private Brush selectedBrush = Brushes.Black; // The current selected brush color
        Color selectedColor = Colors.Black;

        public MainWindow()
        {
            InitializeComponent();
            ToolBar toolbar = (ToolBar)this.FindName("Toolbar");

        }

        // Start drawing when the mouse button is pressed
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                RadioButton button = WhatRadioButton();

                switch (button.Name)
                {
                    case "LinearGradiant":
                        selectedBrush = new LinearGradientBrush(Colors.Red, Colors.Black, 45);
                        break;
                    case "RadialGradient":
                        selectedBrush = new RadialGradientBrush(Colors.Red, Colors.Blue);
                        break;
                    case "Eraser":
                        selectedBrush = Brushes.White; // You can modify this if needed
                        break;
                }
                int thickness = Convert.ToInt32(BrushSizeComboBox.SelectedItem);
                Ellipse currentDot = new Ellipse
                {
                    Fill = selectedBrush,  // Use selected brush
                    Width = thickness,     // Width of the dot
                    Height = thickness     // Height of the dot (same as width for a perfect circle)
                };
                var position = e.GetPosition(DrawingCanvas); // Get initial mouse position
                Canvas.SetLeft(currentDot, position.X - thickness / 2);
                Canvas.SetTop(currentDot, position.Y - thickness / 2);
                DrawingCanvas.Children.Add(currentDot);
                currentLine = new Polyline
                {
                    Stroke = new SolidColorBrush(selectedColor),
                    StrokeThickness = thickness
                };
                DrawingCanvas.Children.Add(currentLine);
                var positon = e.GetPosition(DrawingCanvas);
                currentLine.Points.Add(position);
            }
        }

        // Draw the line while the mouse is moving
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                var position = e.GetPosition(DrawingCanvas);
                currentLine.Points.Add(position);
            }
        }

        // Stop drawing when the mouse button is released
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }

        // Color Picker selection changed
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            selectedColor = (Color)e.NewValue;
            selectedBrush = new SolidColorBrush(selectedColor);
        }

        // Determine which radio button is selected
        private RadioButton WhatRadioButton()
        {
            foreach (var child in Toolbar.Items)
            {
                if (child is RadioButton radioButton && radioButton.IsChecked == true)
                {
                    return radioButton;
                }
            }
            return null;
        }
    }
}
