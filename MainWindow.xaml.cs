using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using PeakyPaint;
using System.Configuration;
using System.Formats.Asn1;


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
        private DrawingUtensil utensil;
        private Ellipse MouseIcon; // Declare the MouseIcon ellipse
        private Cloudsaves cloudsaves = new Cloudsaves();

        public MainWindow()
        {
            InitializeComponent();
            utensil = new(DrawingCanvas);
            // Create the MouseIcon ellipse
            MouseIcon = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Red),
                Width = 20,
                Height = 20,
                Opacity = 0.2
            };

            // Add the ellipse to the Canvas
            DrawingCanvas.Children.Add(MouseIcon);

        }

        // Start drawing when the mouse button is pressed
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;

            if (e.ButtonState == MouseButtonState.Pressed)
            {


                position = e.GetPosition(DrawingCanvas); // Get initial mouse position

                RadioButton button = WhatRadioButton();

                switch (button.Name)
                {
                    case "LinearGradiant":
                        selectedbrush = new LinearGradientBrush(Colors.Red, Colors.Black, 45);
                        break;
                    case "RadialGradient":                                                      //moeten kleuren kunnen kiezen
                        selectedbrush = new RadialGradientBrush(Colors.Red, Colors.Blue);
                        break;
                    case "Eraser":
                        selectedbrush = Brushes.White; //moet background colour zijn
                        break;
                    default:
                        selectedbrush = new SolidColorBrush(Colors.Black);
                        break;

                }
                currentLine = utensil.Line(selectedbrush, thickness);
                SetDot(selectedbrush, thickness, position);

                DrawingCanvas.Children.Add(currentLine); // Add line to canvas

                currentLine.Points.Add(position);  // Add first point to the line
            }
        }


        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            position = e.GetPosition(DrawingCanvas);
            Canvas.SetLeft(MouseIcon, position.X - MouseIcon.Width / 2);
            Canvas.SetTop(MouseIcon, position.Y - MouseIcon.Height / 2);

            if (isDrawing && currentLine != null)  // Only draw if mouse button is pressed
            {
                currentLine.Points.Add(position);
                //SetDot(selectedbrush, thickness, position);

            }
        }

        // Stop drawing when the mouse button is released
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            if (selectedbrush.GetType() == typeof(SolidColorBrush))
            {
                SetDot(selectedbrush, thickness, position);
            }

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
            // Dynamically create a RadioButton for SolidColorBrush (if no other is checked)
            RadioButton defaultButton = new RadioButton
            {
                Name = "SolidcolorBrush",
                Content = "SolidcolorBrush"
            };
            return defaultButton;
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
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Get the actual width and height of the Canvas
            double canvasWidth = DrawingCanvas.ActualWidth;
            double canvasHeight = DrawingCanvas.ActualHeight;

            // Create a RenderTargetBitmap to capture the Canvas area
            RenderTargetBitmap renderTargetBitmap = new(
                (int)canvasWidth,
                (int)canvasHeight,
                96, 96, PixelFormats.Pbgra32);

            // Render only the Canvas content (no toolbar or surrounding UI)
            renderTargetBitmap.Render(DrawingCanvas);

            // Save to a memory stream
            using MemoryStream memoryStream = new();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Save the frame into the memory stream
            encoder.Save(memoryStream);

            // Convert to System.Drawing.Bitmap
            memoryStream.Seek(0, SeekOrigin.Begin);
            using System.Drawing.Bitmap bitmap = new(memoryStream);
            Microsoft.Win32.SaveFileDialog saveFileDialog = new()
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
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Filter = "Bitmap Files (*.bmp)|*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Load the selected bitmap from the file
                string filePath = openFileDialog.FileName;
                System.Drawing.Bitmap loadedBitmap = new(filePath);

                // Convert the System.Drawing.Bitmap to a WPF BitmapImage
                BitmapImage bitmapImage = new();
                using (MemoryStream memoryStream = new())
                {
                    loadedBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }

                // Create an Image control to display the loaded bitmap
                Image imageControl = new()
                {
                    Source = bitmapImage,
                    Stretch = Stretch.None
                };

                // Add the image control to the Canvas, with the same offset
                double cropOffset = 0; // This should match the saved offset
                Canvas.SetTop(imageControl, cropOffset); // Place the image 100px from the top of the Canvas
                DrawingCanvas.Children.Add(imageControl);
                MouseIcon = new Ellipse
                {
                    Fill = new SolidColorBrush(Colors.Red),
                    Width = 20,
                    Height = 20,
                    Opacity = 0.2
                };
                DrawingCanvas.Children.Add(MouseIcon);
                
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

            MouseIcon.Visibility = Visibility.Collapsed;

            // Render the Canvas into the bitmap
            renderTargetBitmap.Render(DrawingCanvas);

            // Show a SaveFileDialog to choose where to save the file
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|AVIF Files (*.avif)|*.avif"
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

        private async Task SaveMenuItem_Click(string filepath)
        {
            try
            {
                // Get the actual width and height of the Canvas
                double canvasWidth = DrawingCanvas.ActualWidth;
                double canvasHeight = DrawingCanvas.ActualHeight;

                // Define a temp file path
                string tempFilePath = filepath;

                // Create a RenderTargetBitmap to capture the Canvas area
                RenderTargetBitmap renderTargetBitmap = new(
                    (int)canvasWidth,
                    (int)canvasHeight,
                    96, 96, PixelFormats.Pbgra32);

                // Render only the Canvas content (no toolbar or surrounding UI)
                renderTargetBitmap.Render(DrawingCanvas);

                // Save to a memory stream
                using MemoryStream memoryStream = new();
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                // Save the frame into the memory stream
                encoder.Save(memoryStream);

                // Save the memory stream to the temp file
                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }

                // Confirm file exists
                if (File.Exists(tempFilePath))
                {
                    MessageBox.Show($"File saved to: {tempFilePath}");
                }
                else
                {
                    throw new FileNotFoundException("Temp file was not created.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
            MouseIcon.Visibility = Visibility.Visible;
        }



        private void DrawingCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseIcon.Visibility = Visibility.Collapsed;
            DrawingCanvas.MouseDown -= Canvas_MouseDown;
        }

        private void DrawingCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseIcon.Visibility = Visibility.Visible;
            DrawingCanvas.MouseDown += Canvas_MouseDown;
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "temp_file.png");

            // Save the menu item
            await SaveMenuItem_Click(filepath);

            // Upload to cloud saves and export icon
            //cloudsaves.UploadButton_Click(sender, e, filepath);
            await cloudsaves.ExportIcon(DrawingCanvas, filepath);
        }

    }
}