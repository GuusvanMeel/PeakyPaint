using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Win32; // For OpenFileDialog and SaveFileDialog

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private bool isDrawing = false; // Flag to track drawing state
        private Polyline currentLine; // The current line being drawn

        public MainWindow()
        {
            InitializeComponent();
        }

        // Start drawing when the mouse button is pressed
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                isDrawing = true;
                int thickness = Convert.ToInt32(BrushSizeComboBox.SelectedItem);
                currentLine = new Polyline
                {
                    Stroke = Brushes.Black,   // Black color for drawing
                    StrokeThickness = thickness       // Line thickness
                };
                DrawingCanvas.Children.Add(currentLine); // Add line to canvas
                var position = e.GetPosition(DrawingCanvas); // Get initial mouse position
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                // Add file saving logic here, e.g., save the current drawing to the file
                MessageBox.Show($"Saving file: {filePath}");
            }
        }
    }
}
