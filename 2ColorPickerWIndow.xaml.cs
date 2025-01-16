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

namespace PeakyPaint
{
    
    public partial class _2ColorPickerWIndow : Window
    {
       private readonly string  button;
        private readonly MainWindow window;
        public _2ColorPickerWIndow(string _buttonname, MainWindow _window)
        {
            InitializeComponent();
            button = _buttonname;
            window = _window;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
            if (ColorPicker1.SelectedColor != null && ColorPicker2.SelectedColor != null)
            {
                window.GradientBrush((Color)ColorPicker1.SelectedColor, (Color)ColorPicker2.SelectedColor, button);
            }
            else if (ColorPicker1.SelectedColor == ColorPicker2.SelectedColor)
            {
                MessageBox.Show("Please pick 2 colors that are different from each other");
            }
            else
            {
                MessageBox.Show("Please pick 2 colors");
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ColorPicker2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ColorPicker1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            
        }
    }
}
