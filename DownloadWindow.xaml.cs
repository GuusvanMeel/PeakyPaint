using DrawingApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace PeakyPaint
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        MainWindow mainwindow;
        Cloudsaves cloudsaves;
        private List<string> files = new List<string>();
        public DownloadWindow(MainWindow _mainwindow)
        {
            InitializeComponent();
            mainwindow = _mainwindow;
            cloudsaves = new(mainwindow, this);
            refreshImages();

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 0)  // Check if bmpName is not null
            {
                string bmpName = files[0];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 1)  // Check if bmpName is not null
            {
                string bmpName = files[1];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button3_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 2)  // Check if bmpName is not null
            {
                string bmpName = files[2];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button4_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 3)  // Check if bmpName is not null
            {
                string bmpName = files[3];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button5_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 4)  // Check if bmpName is not null
            {
                string bmpName = files[4];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button6_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 5)  // Check if bmpName is not null
            {
                string bmpName = files[5];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button7_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 6)  // Check if bmpName is not null
            {
                string bmpName = files[6];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button8_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 7)  // Check if bmpName is not null
            {
                string bmpName = files[7];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }

        private async void Button9_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count > 8)  // Check if bmpName is not null
            {
                string bmpName = files[8];
                if (!System.IO.Path.GetExtension(bmpName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    bmpName = System.IO.Path.ChangeExtension(bmpName, ".bmp");
                }
                await cloudsaves.DownloadFileFromNextcloud(bmpName, this);
                this.Close();
            }
        }


        private async void refreshImages()
        {
            await cloudsaves.GetFilesFromNextcloud();
        }

        public async Task FillNextImage(string buttonName, string imageName)
        {
            // Find the button by its name
            string imagePath = mainwindow.PeakyPaintDir + imageName;
            Button button = (Button)this.FindName(buttonName);
            if (button != null)
            {
                // Set the image source
                Image image = (Image)button.Template.FindName("ImageBackground", button);
                if (image != null && image.Source == null && File.Exists(imagePath))
                {
                    // Set the source of the image on the button
                    image.Source = new BitmapImage(new Uri(imagePath));
                    files.Add(imageName);
                    cloudsaves.imageCount++;
                }
            }
        }


    }
}
