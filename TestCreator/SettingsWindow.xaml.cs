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
using TestCreator.Properties;

namespace TestCreator
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SliderFontSize.ValueChanged += FontSizeChange;
        }

        private void MainColorCange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void MainColorGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((MainColorCB.SelectedItem as ComboBoxItem).Tag as string))
            {
                case "blue":
                    Singlton.Color2 = new SolidColorBrush(new Color { R = 38, G = 0, B = 230, A = 255 });
                    Singlton.Color1 = new SolidColorBrush(new Color { R = 96, G = 69, B = 230, A = 255 });
                    Singlton.ColorF = new SolidColorBrush(Colors.White);
                    break;
                case "red":
                    Singlton.Color2 = new SolidColorBrush(new Color { R = 255, G = 33, B = 36, A = 255 });
                    Singlton.Color1 = new SolidColorBrush(new Color { R = 163, G = 21, B = 23, A = 255 });
                    Singlton.ColorF = new SolidColorBrush(Colors.White);
                    break;
                case "green":
                    Singlton.Color2 = new SolidColorBrush(new Color { R = 0, G = 255, B = 50, A = 255 });
                    Singlton.Color1 = new SolidColorBrush(new Color { R = 40, G = 255, B = 90, A = 255 });
                    Singlton.ColorF = new SolidColorBrush(Colors.Black);
                    break;
                case "yellow":
                    Singlton.Color2 = new SolidColorBrush(new Color { R = 232, G = 255, B = 0, A = 255 });
                    Singlton.Color1 = new SolidColorBrush(new Color { R = 240, G = 255, B = 77, A = 255 });
                    Singlton.ColorF = new SolidColorBrush(Colors.Black);
                    break;
                default:
                    string colorizationValue = string.Format("{0:x}", Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", "00000000"));
                    Color color = (Color)ColorConverter.ConvertFromString("#" + colorizationValue);
                    color.A = 100;
                    Singlton.Color1 = new SolidColorBrush(color);
                    color.A = 255;
                    Singlton.Color2 = new SolidColorBrush(color);
                    float r = color.R, g = color.G, b = color.B;
                    float s, v;
                    Singlton.RGBtoHSV(r, g, b, out _, out s, out v);
                    if (s >= 191 || v >= 191)
                    {
                        Singlton.ColorF = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        Singlton.ColorF = new SolidColorBrush(Colors.White);
                    }
                    break;
            }
        }

        private void FontSizeChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Singlton.SizeF = e.NewValue;
        }
    }
}
