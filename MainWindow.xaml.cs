using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private bool isDrawing = false; // Flag to track drawing state
        private Polyline? currentLine = null; // The current line being drawn
        Color selectedColor = Colors.Black;
        double zoomFactor = 1.0;

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
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Get the actual width and height of the Canvas
            double canvasWidth = DrawingCanvas.ActualWidth;
            double canvasHeight = DrawingCanvas.ActualHeight;

            // Create a RenderTargetBitmap to capture the Canvas area
            RenderTargetBitmap renderTargetBitmap = new (
                (int)canvasWidth,
                (int)canvasHeight,
                96, 96, PixelFormats.Pbgra32);

            // Render only the Canvas content (no toolbar or surrounding UI)
            renderTargetBitmap.Render(DrawingCanvas);

            // Save to a memory stream
            using MemoryStream memoryStream = new ();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Save the frame into the memory stream
            encoder.Save(memoryStream);

            // Convert to System.Drawing.Bitmap
            memoryStream.Seek(0, SeekOrigin.Begin);
            using System.Drawing.Bitmap bitmap = new (memoryStream);
            Microsoft.Win32.SaveFileDialog saveFileDialog = new ()
            {
                Filter = "Bitmap Files (*.bmp)|*.bmp"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select the saved bitmap
            Microsoft.Win32.OpenFileDialog openFileDialog = new ()
            {
                Filter = "Bitmap Files (*.bmp)|*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Load the selected bitmap from the file
                string filePath = openFileDialog.FileName;
                System.Drawing.Bitmap loadedBitmap = new (filePath);

                // Convert the System.Drawing.Bitmap to a WPF BitmapImage
                BitmapImage bitmapImage = new ();
                using (MemoryStream memoryStream = new ())
                {
                    loadedBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }

                // Create an Image control to display the loaded bitmap
                Image imageControl = new ()
                {
                    Source = bitmapImage,
                    Stretch = Stretch.None
                };

                // Add the image control to the Canvas, with the same offset
                double cropOffset = 0; // This should match the saved offset
                Canvas.SetTop(imageControl, cropOffset); // Place the image 100px from the top of the Canvas
                DrawingCanvas.Children.Add(imageControl);
            }
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Define the size of the area to capture (same size as the Canvas)
            double width = DrawingCanvas.ActualWidth;
            double height = DrawingCanvas.ActualHeight;


            // Create a RenderTargetBitmap to render the Canvas content into a bitmap
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)width, (int)height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            // Render the Canvas into the bitmap
            renderTargetBitmap.Render(DrawingCanvas);

            // Show a SaveFileDialog to choose where to save the file
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Create a file stream to save the image to
                using (System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                {
                    // Choose the appropriate encoder based on the file extension
                    BitmapEncoder encoder = saveFileDialog.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                        ? new PngBitmapEncoder()
                        : new BmpBitmapEncoder();

                    // Add the frame (image) to the encoder
                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                    // Save the image to the selected file
                    encoder.Save(fs);
                }
            }
        }

        //zoom
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                
                if (e.Key == Key.Zoom)
                { 
                    ZoomIn();
                }
                else if (e.Key == Key.Subtract) 
                {
                    ZoomOut();
                }
            }
        }

        // Zoom in and out using the mouse wheel
        private void DrawingCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) // Mouse wheel up
            {
                ZoomIn();
            }
            else if (e.Delta < 0) // Mouse wheel down
            {
                ZoomOut();
            }
        }

        // Zoom In
        private void ZoomIn()
        {
            zoomFactor *= 1.1; // Increase zoom by 10%
            UpdateCanvasZoom();
        }

        // Zoom Out
        private void ZoomOut()
        {
            zoomFactor /= 1.1; // Decrease zoom by 10%
            UpdateCanvasZoom();
        }

        // Update the scaling transform
        private void UpdateCanvasZoom()
        {
            CanvasScaleTransform.ScaleX = zoomFactor;
            CanvasScaleTransform.ScaleY = zoomFactor;
        }
        private void DiagramDesignerCanvasContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Determine the direction of the zoom (in or out)
            bool zoomIn = e.Delta > 0;

            // Set the scale value based on the direction of the zoom
            zoomFactor += zoomIn ? 0.1 : -0.1;

            // Set the maximum and minimum scale values
            zoomFactor = zoomFactor < 0.1 ? 0.1 : zoomFactor;
            zoomFactor = zoomFactor > 10.0 ? 10.0 : zoomFactor;

            
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
    }


}

