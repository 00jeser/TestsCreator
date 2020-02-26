using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace TestCreator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Task> lst = JsonConvert.DeserializeObject<List<Task>>(File.ReadAllText("txt.json"));

        public bool someSelect = false;

        public MainWindow()
        {
            InitializeComponent();

            tasksList.ItemsSource = lst;
            try
            {
                string colorizationValue = string.Format("{0:x}", Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", "00000000"));
                Color color = (Color)ColorConverter.ConvertFromString("#" + colorizationValue);
                color.A = 200;
                float r = color.R, g = color.G, b = color.B, h = 0, s = 0, v = 0;
                Singlton.RGBtoHSV(r, g, b, out h, out s, out v);
                s = 1;
                Singlton.HSVtoRGB(h, s, v, out r, out g, out b);
                color.R = (byte)r; color.G = (byte)g; color.B = (byte)b;
                this.Resources["systemColor"] = new SolidColorBrush(color);
                s = 120;
                Singlton.HSVtoRGB(h, s, v, out r, out g, out b);
                color.R = (byte)r; color.G = (byte)g; color.B = (byte)b;
                this.Resources["lightSystemColor"] = new SolidColorBrush(color);
            }
            catch{}
        }

        private void close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void maximize(object sender, RoutedEventArgs e)
        {
            this.WindowState =this.WindowState == WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
        }

        private void minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Width >= 540) ProjName.Visibility = Visibility.Visible; else ProjName.Visibility = Visibility.Hidden;
            if (this.WindowState == WindowState.Maximized) mainBorder.BorderThickness = new Thickness(7.2); else mainBorder.BorderThickness = new Thickness(0);
        }

        private void tasksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (tasksList.SelectedItem as Task).select = true;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tasksList.Focus();
        }

        private void focusList(object sender, RoutedEventArgs e)
        {
            tasksList.Focus();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            tasksList.Focus();
        }
    }
}
