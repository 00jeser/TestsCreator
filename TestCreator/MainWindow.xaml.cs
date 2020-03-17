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
using System.Diagnostics;

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
            EditButton.IsEnabled = tasksList.SelectedIndex != -1;
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

        private void EditItem(object sender, RoutedEventArgs e)
    {
            var i = (Task)tasksList.SelectedItem;
            if (i.type)
            {
                Edit1Grid.Visibility = Visibility.Visible;
                E1TText.Text = i.task;
                E1TVars.ItemsSource = i.visualVars;
            }
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (tasksList.SelectedIndex != -1)
            {
                Singlton.tasks.RemoveAt(tasksList.SelectedIndex);
                tasksList.ItemsSource = "";
                tasksList.ItemsSource = Singlton.tasks;
            }
        }

        private void EditGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Edit1Grid.Visibility = Visibility.Hidden;
            EditSomeGrid.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vars = new List<Variable>();
            foreach (VisualVars i in E1TVars.Items) 
            {
                Variable v = new Variable();
                if (i.Value.IndexOf(';') != -1)
                {
                    v.lst = i.Value.Split().Select(s => s.Trim()).ToArray();
                }
                else 
                {
                    v.Range = i.Value;
                }
                vars.Add(v);
            }
            Debug.WriteLine(tasksList.SelectedIndex);
            Singlton.tasks[tasksList.SelectedIndex].vars = vars.ToArray();
        }
    }
}
