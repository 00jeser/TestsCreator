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
using TestCreator.Properties;

namespace TestCreator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool someSelect = false;

        public MainWindow()
        {
            InitializeComponent();

            tasksList.ItemsSource = Singlton.tasks;
            Singlton.StyleChanged += Singlton_StyleChanged;
            Singlton.FontColorChanged += Singlton_FontColorChanged;
            Singlton.FontSizeChanged += Singlton_FontSizeChanged;
            try
            {
                this.Resources["Color1"] = Settings.Default["Color1"];
                this.Resources["Color2"] = Settings.Default["Color2"];
                this.Resources["ColorF"] = Settings.Default["ColorF"];
            }
            catch { }
        }

        private void Singlton_FontSizeChanged(object v)
        {
            this.Resources["FontSize"] = v;
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.Show();
            sw.Focus();
        }

        private void Singlton_FontColorChanged(object v)
        {
            this.Resources["ColorF"] = v;
        }

        private void Singlton_StyleChanged(SolidColorBrush color1, SolidColorBrush color2)
        {
            this.Resources["Color1"] = color1;
            this.Resources["Color2"] = color2;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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
            if (sender != sett)
                tasksList.Focus();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            tasksList.Focus();
        }
    }
}
