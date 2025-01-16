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
        public string nextcloudUrl = "http://192.168.128.149/remote.php/dav/files/admin/Peakypaint/Global/";
        private readonly string username = "admin";
        private readonly string password = "Welkom123!";
        public async Task UploadButton_Click(string _filepath)
        {

                string filePath = _filepath;
                string fileName = Path.GetFileName(filePath);

                try
                {
                    // Upload the file
                    await UploadFileToNextcloud(filePath, fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
            }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        public async Task UploadFileToNextcloud(string localFilePath, string remoteFileName)
        {
            string remoteUrl = $"{nextcloudUrl}{remoteFileName}";

            using HttpClient client = new ();

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

        public async Task ExportIcon(Canvas DrawingCanvas, string _filepath)
        {
            // Use ActualWidth and ActualHeight to ensure proper dimensions
            int previewWidth = (int)DrawingCanvas.ActualWidth;
            int previewHeight = (int)DrawingCanvas.ActualHeight;

            if (previewWidth <= 0 || previewHeight <= 0)
            {
                MessageBox.Show("Canvas dimensions are invalid. Ensure the Canvas has been rendered.");
                return;
            }

            // Create a RenderTargetBitmap to render the Canvas content
            RenderTargetBitmap renderTargetBitmap = new (
                previewWidth, previewHeight, 96, 96, PixelFormats.Pbgra32);

            // Ensure the Canvas is properly measured and arranged
            DrawingCanvas.Measure(new Size(previewWidth, previewHeight));
            DrawingCanvas.Arrange(new Rect(0, 0, previewWidth, previewHeight));

            // Render the Canvas into the bitmap
            renderTargetBitmap.Render(DrawingCanvas);

            // Save the image to the file
            using FileStream fs = new (_filepath, FileMode.Create);
            BitmapEncoder encoder = _filepath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                ? new PngBitmapEncoder()
                : new JpegBitmapEncoder { QualityLevel = 50 };

            // Add the frame (image) to the encoder
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Save the image
            encoder.Save(fs);

            // Upload the file
            
        }

    }

}
