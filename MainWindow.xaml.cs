using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private bool isDrawing = false; // Flag to track drawing state
        private Polyline currentLine; // The current line being drawn
        private System.Windows.Media.Brush selectedBrush = System.Windows.Media.Brushes.Black; // The current selected brush color
        System.Windows.Media.Color selectedColor = Colors.Black;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Start drawing when the mouse button is pressed
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Brush selectedbrush = new SolidColorBrush(selectedColor);
                int thickness = Convert.ToInt32(BrushSizeComboBox.SelectedItem);
                var position = e.GetPosition(DrawingCanvas); // Get initial mouse position
                RadioButton? button = WhatRadioButton();
                if (button != null)
                {
                    switch (button.Name)
                    {
                        case "LinearGradiant":
                            selectedbrush = new LinearGradientBrush(Colors.Red, Colors.Black, 45); //HIER OOK KLEURENPICKER
                            break;
                        case "RadialGradient":
                            selectedbrush = new RadialGradientBrush(Colors.Red, Colors.Blue); // JE HEBT EEN KLEURENPICKER VOOR DIT NODIG
                            break;
                        case "Eraser":
                            selectedbrush = Brushes.White; //MOET VERANDEREN IN BACKGROUND COLOUR NIET WIT
                            break;



                    }
                }
                
                SetDot(selectedbrush, thickness, position);

                currentLine = new Polyline
                {
                    Stroke = new SolidColorBrush(selectedColor),
                    StrokeThickness = thickness
                };
                
                DrawingCanvas.Children.Add(currentLine); // Add line to canvas
              
                
                currentLine.Points.Add(position);  // Add first point to the line
            }
        }

        // Draw the line while the mouse is moving
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && currentLine != null)  // Only draw if mouse button is pressed
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
            selectedColor = (System.Windows.Media.Color)e.NewValue;
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
        private void SetDot(Brush selectedbrush, Double thickness, Point position)
        {
            Ellipse currentDot = new()
            {
                Fill = selectedbrush,  // Use selected brush
                Width = thickness,     // Width of the dot
                Height = thickness     // Height of the dot (same as width for a perfect circle)
            };

            Canvas.SetLeft(currentDot, position.X - thickness / 2);
            Canvas.SetTop(currentDot, position.Y - thickness / 2);
            DrawingCanvas.Children.Add(currentDot);
        }
    }
}
