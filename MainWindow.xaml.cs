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
                        selectedBrush = System.Windows.Media.Brushes.White; // You can modify this if needed
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

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            renderTargetBitmap.Render(DrawingCanvas);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                // Save the frame into a memory stream
                encoder.Save(memoryStream);

                // Convert to System.Drawing.Bitmap
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(memoryStream))
                {
                    Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Bitmap Files (*.bmp)|*.bmp"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
            }
        }
                private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Open an OpenFileDialog to select the image file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Bitmap Files (*.bmp)|*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Create a BitmapImage from the selected file
                BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));

                // Create an Image control to display the loaded image
                System.Windows.Controls.Image loadedImage = new System.Windows.Controls.Image
                {
                    Source = bitmapImage,
                    Width = DrawingCanvas.ActualWidth,
                    Height = DrawingCanvas.ActualHeight
                };

                // Clear the canvas and add the image to it
                DrawingCanvas.Children.Clear();
                DrawingCanvas.Children.Add(loadedImage);
            }
        }

    }
}
