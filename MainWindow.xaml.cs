using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using Haley.Models;

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private bool isDrawing = false; // Flag to track drawing state
        private Polyline? currentLine = null; // The current line being drawn
        private Color selectedColor = Colors.Black;
        private int thickness = 20;
        private Brush selectedbrush = new SolidColorBrush(Colors.Black);
        private Point position;
        private double zoomFactor = 1.0;

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
                selectedbrush = new SolidColorBrush(selectedColor);

                position = e.GetPosition(DrawingCanvas); // Get initial mouse position

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
                    Stroke = selectedbrush,
                    StrokeThickness = thickness
                };

                DrawingCanvas.Children.Add(currentLine); // Add line to canvas

                currentLine.Points.Add(position); // Add first point to the line
            }
        }

        // Draw the line while the mouse is moving
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            position = e.GetPosition(DrawingCanvas);

            if (isDrawing && currentLine != null) // Only draw if mouse button is pressed
            {
                currentLine.Points.Add(position);
            }
        }

        // Stop drawing when the mouse button is released
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }

        // Color Picker selection changed
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                selectedColor = e.NewValue.Value;
            }
        }

        // Determine which radio button is selected
        private RadioButton? WhatRadioButton()
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

        private void SetDot(Brush selectedbrush, double thickness, Point position)
        {
            Ellipse currentDot = new()
            {
                Fill = selectedbrush,
                Width = thickness,
                Height = thickness
            };

            Canvas.SetLeft(currentDot, position.X - thickness / 2);
            Canvas.SetTop(currentDot, position.Y - thickness / 2);
            DrawingCanvas.Children.Add(currentDot);
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Save canvas content logic...
        }

        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Load bitmap to canvas logic...
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Export canvas to image logic...
        }

        // Zoom In
        private void ZoomIn()
        {
            zoomFactor *= 1.1;
            UpdateCanvasZoom();
        }

        // Zoom Out
        private void ZoomOut()
        {
            zoomFactor /= 1.1;
            UpdateCanvasZoom();
        }

        private void UpdateCanvasZoom()
        {
            CanvasScaleTransform.ScaleX = zoomFactor;
            CanvasScaleTransform.ScaleY = zoomFactor;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderValueText.Text = $"Zoom: {Math.Round(e.NewValue * 100)}%";
            if (CanvasScaleTransform != null)
            {
                CanvasScaleTransform.ScaleX = e.NewValue;
                CanvasScaleTransform.ScaleY = e.NewValue;
            }
        }

        private void BrushSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MouseIcon != null)
            {
                thickness = Convert.ToInt32(BrushSizeComboBox.SelectedItem);
                MouseIcon.Width = thickness;
                MouseIcon.Height = thickness;
            }
        }
    }
}
