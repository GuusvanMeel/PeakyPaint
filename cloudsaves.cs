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
using DrawingApp;
using System.Xml.Linq;

namespace PeakyPaint
{
    internal class Cloudsaves
    {
        DownloadWindow DownloadWindow;
        public string nextcloudUrl = "http://192.168.128.149/remote.php/dav/files/admin/Peakypaint/Global/";
        private string username = "admin";
        private string password = "Welkom123!";
        private MainWindow mainwindow;
        public int imageCount = 1;



        public Cloudsaves(MainWindow _mainwindow, DownloadWindow _downloadwindow)
        {
            mainwindow = _mainwindow;
            DownloadWindow = _downloadwindow;
        }
        public Cloudsaves(MainWindow _mainwindow)
        {
            mainwindow = _mainwindow;
        }
        public async Task UploadButton_Click(object sender, RoutedEventArgs e, string _filepath)
        {

            string filePath = mainwindow.PeakyPaintDir + _filepath;
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

        public async Task ExportIcon(Canvas DrawingCanvas, string _filepath)
        {
            // Use ActualWidth and ActualHeight to ensure proper dimensions
            int previewWidth = (int)DrawingCanvas.ActualWidth;
            int previewHeight = (int)DrawingCanvas.ActualHeight;

            _filepath = mainwindow.PeakyPaintDir + _filepath;

            if (previewWidth <= 0 || previewHeight <= 0)
            {
                MessageBox.Show("Canvas dimensions are invalid. Ensure the Canvas has been rendered.");
                return;
            }

            // Create a RenderTargetBitmap to render the Canvas content
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                previewWidth, previewHeight, 96, 96, PixelFormats.Pbgra32);

            // Ensure the Canvas is properly measured and arranged
            DrawingCanvas.Measure(new Size(previewWidth, previewHeight));
            DrawingCanvas.Arrange(new Rect(0, 0, previewWidth, previewHeight));

            // Render the Canvas into the bitmap
            renderTargetBitmap.Render(DrawingCanvas);

            // Save the image to the file
            using FileStream fs = new FileStream(_filepath, FileMode.Create);
            BitmapEncoder encoder = _filepath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                ? new PngBitmapEncoder()
                : new JpegBitmapEncoder { QualityLevel = 50 };

            // Add the frame (image) to the encoder
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Save the image
            encoder.Save(fs);

            // Upload the file

        }

        public async Task DownloadFileFromNextcloud(string filename, DownloadWindow _downloadwindow)
        {
            DownloadWindow = _downloadwindow;
            using HttpClient client = new HttpClient();

            string remoteFileUrl = nextcloudUrl + filename;

            string localFilePath = mainwindow.PeakyPaintDir + filename;

            // Set up authentication
            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                // Send a GET request to download the file
                HttpResponseMessage response = await client.GetAsync(remoteFileUrl);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Error downloading file: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Read the file content and save it locally
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(localFilePath, fileBytes);


                mainwindow.LoadMenuItem_Click(localFilePath);
                //MessageBox.Show($"File downloaded successfully: {localFilePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task DownloadIconFromNextcloud(string filename, DownloadWindow _downloadwindow)
        {
            DownloadWindow = _downloadwindow;
            using HttpClient client = new HttpClient();

            string remoteFileUrl = nextcloudUrl + filename;

            string localFilePath = mainwindow.PeakyPaintDir + filename;

            // Set up authentication
            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                // Send a GET request to download the file
                HttpResponseMessage response = await client.GetAsync(remoteFileUrl);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Error downloading file: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Read the file content and save it locally
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(localFilePath, fileBytes);

                //MessageBox.Show($"File downloaded successfully: {localFilePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task GetFilesFromNextcloud()
        {
            try
            {
                var files = await ListFilesFromNextcloud();

                // Filter for PNG files
                var jpgFiles = files
                    .Where(f => f.EndsWith(".jpg"))
                    .OrderByDescending(f => f)  // Sort files based on filename (assuming it contains a timestamp)
                    .Take(9)  // Get the 9 most recent PNG files
                    .ToList();

                // Display the 9 most recent files
                foreach (var file in jpgFiles)
                {
                    await DownloadIconFromNextcloud(file, DownloadWindow);
                    await DownloadWindow.FillNextImage($"Button{imageCount}", file);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // List files from Nextcloud using WebDAV
        private async Task<List<string>> ListFilesFromNextcloud()
        {
            using HttpClient client = new HttpClient();

            // Set up the authentication header
            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // WebDAV PROPFIND request to list files in the directory
            var request = new HttpRequestMessage(CustomHttpMethods.Propfind, nextcloudUrl)
            {
                Content = new StringContent("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<d:propfind xmlns:d=\"DAV:\">\n  <d:prop>\n    <d:displayname/>\n    <d:getlastmodified/>\n  </d:prop>\n</d:propfind>", Encoding.UTF8, "text/xml")
            };

            // Set the depth header to 1 to list files (not folders)
            request.Headers.Add("Depth", "1");

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve files: {response.StatusCode} - {response.ReasonPhrase}");
            }

            // Parse the response to get file names and last modified timestamps
            string responseContent = await response.Content.ReadAsStringAsync();
            XDocument xmlDoc = XDocument.Parse(responseContent);

            var files = xmlDoc.Descendants("{DAV:}response")
                              .Where(r => r.Descendants("{DAV:}displayname").Any())
                              .Select(r => r.Descendants("{DAV:}displayname").First().Value)
                              .ToList();

            return files;
        }
    }

    public class CustomHttpMethods
    {
        // Create a custom HttpMethod for PROPFIND
        public static readonly HttpMethod Propfind = new HttpMethod("PROPFIND");
    }

}

