using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace PeakyPaint
{
    internal class Cloudsaves
    {
        private string nextcloudUrl = "http://192.168.128.149/remote.php/dav/files/admin/Peakypaint/Global/";
        private string username = "admin";
        private string password = "Welkom123!";
        public async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a file dialog to select the file
            //Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            //if (openFileDialog.ShowDialog() == true)
            //{
            //string filePath = openFileDialog.FileName;
                string filePath = Path.Combine(Path.GetTempPath(), $"temp_file.bmp");
                string fileName = Path.GetFileName(filePath);

                try
                {
                    // Upload the file
                    await UploadFileToNextcloud(filePath, fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        //}

        private async Task UploadFileToNextcloud(string localFilePath, string remoteFileName)
        {
            string remoteUrl = $"{nextcloudUrl}{remoteFileName}";

            using HttpClient client = new HttpClient();

            // Set up the authentication
            var byteArray = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Read the file content
            byte[] fileBytes = await File.ReadAllBytesAsync(localFilePath);
            using var content = new ByteArrayContent(fileBytes);

            try
            {
                // Send a PUT request to upload the file
                HttpResponseMessage response = await client.PutAsync(remoteUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("File uploaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Error uploading file: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExportPreview_Click(Canvas DrawingCanvas)
        {
            // Define the desired preview dimensions (e.g., 200x200 pixels)
            int previewWidth = (int)DrawingCanvas.Width;
            int previewHeight = (int)DrawingCanvas.Height;

            // Create a RenderTargetBitmap to render the Canvas content
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                previewWidth, previewHeight, 96, 96, PixelFormats.Pbgra32);

            // Render the Canvas into the bitmap
            DrawingCanvas.Measure(new Size(DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight));
            DrawingCanvas.Arrange(new Rect(0, 0, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight));
            renderTargetBitmap.Render(DrawingCanvas);

            // Show a SaveFileDialog to choose where to save the preview
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);

                // Use a compressed format like JPEG
                BitmapEncoder encoder = saveFileDialog.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    ? new PngBitmapEncoder()
                    : new JpegBitmapEncoder { QualityLevel = 50 }; // Lower quality for smaller file size

                // Add the frame (image) to the encoder
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                // Save the image to the selected file
                encoder.Save(fs);
            }
        }

    }
}