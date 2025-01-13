using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Controls;
using PeakyPaint; // For OpenFileDialog and 

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private bool isDrawing = false; // Flag to track drawing state
        private Polyline? currentLine; // The current line being drawn

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
               

                Brush selectedbrush = Brushes.Black;
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
                int thickness = Convert.ToInt32(BrushSizeComboBox.SelectedItem);
                Ellipse currentDot = new Ellipse
                {
                    Fill = selectedbrush,  // Use selected brush
                    Width = thickness,     // Width of the dot
                    Height = thickness     // Height of the dot (same as width for a perfect circle)
                };
                var position = e.GetPosition(DrawingCanvas); // Get initial mouse position
                Canvas.SetLeft(currentDot, position.X - thickness / 2);
                Canvas.SetTop(currentDot, position.Y - thickness / 2);
                DrawingCanvas.Children.Add(currentDot);
                currentLine = new Polyline
                {   
                    Stroke = selectedbrush,   
                    StrokeThickness = thickness       
                };
                DrawingCanvas.Children.Add(currentLine); // Add line to canvas
              
                
                currentLine.Points.Add(position);  // Add first point to the line
            }
        }

        // Draw the line while the mouse is moving
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)  // Only draw if mouse button is pressed
            {
                var position = e.GetPosition(DrawingCanvas);  // Get the new mouse position
                currentLine.Points.Add(position);  // Add new point to the line
            }
        }

        // Stop drawing when the mouse button is released
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false; // Stop drawing when the mouse is released
        }

        // Clear the canvas
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();  // Clears all elements on the canvas
        }

        // Open menu item click handler
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new ();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                // Add file loading logic here, e.g., load a drawing from the file
                MessageBox.Show($"Opening file: {filePath}");
            }
        }

        // Save menu item click handler
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new ();
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                // Add file saving logic here, e.g., save the current drawing to the file
                MessageBox.Show($"Saving file: {filePath}");
            }
        }
        private RadioButton WhatRadioButton()
        {
            foreach(var child in Toolbar.Items)
            {
                if(child is RadioButton radiobutton && radiobutton.IsChecked == true)
                {
                   
                    return radiobutton;
                }
            }
            return null;
        }
    }
}
