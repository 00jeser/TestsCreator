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
        public string SaveFolder;
        public bool SaveF = true;

        public MainWindow()
        {
            InitializeComponent();
            var s = Environment.OSVersion.Version.Minor;
            if (Environment.OSVersion.Version.Minor == 1)
            {
                g1.Margin = new Thickness(-2);
                g2.Margin = new Thickness(-2);
                g3.Margin = new Thickness(-2);
            }
            ProjName.Text = s.ToString();

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
            if (this.Resources["ColorF"] == null)
                this.Resources["ColorF"] = Brushes.White;
            this.Resources["FontSize"] = Settings.Default.SizeF;
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
            if (this.Width >= 540) ProjName.Visibility = Visibility.Visible; else ProjName.Visibility = Visibility.Hidden;
            if (this.WindowState == WindowState.Maximized) mainBorder.BorderThickness = new Thickness(7); else mainBorder.BorderThickness = new Thickness(0);
        }

        private void tasksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = tasksList.SelectedIndex != -1;
            DeleteButton.IsEnabled = tasksList.SelectedIndex != -1;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tasksList.Focus();
            }
            catch (Exception)
            {

            }
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
                E1TFormula.Text = i.math;
            }
            else
            {
                EditSomeGrid.Visibility = Visibility.Visible;
                ESTTasks.ItemsSource = i.visualTasks;
            }
            LostSave();
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (tasksList.SelectedIndex != -1)
            {
                Singlton.tasks.RemoveAt(tasksList.SelectedIndex);
                tasksList.ItemsSource = "";
                tasksList.ItemsSource = Singlton.tasks;
                LostSave();
            }
        }

        private void EditGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Edit1Grid.Visibility = Visibility.Hidden;
            EditSomeGrid.Visibility = Visibility.Hidden;
        }

        private void E1TEnd(object sender, RoutedEventArgs e)
        {
            var vars = new List<Variable>();
            foreach (object i in E1TVars.Items)
            {
                if (i is VisualVars)
                {
                    VisualVars ii = (i as VisualVars);
                    Variable v = new Variable();
                    v.Name = ii.Name;
                    if (ii.Value.IndexOf(';') != -1)
                    {
                        v.lst = ii.Value.Split(';').Select(s => s.Trim()).ToArray();
                    }
                    else
                    {
                        v.Range = ii.Value;
                    }
                    vars.Add(v);
                }
            }
            Debug.WriteLine(tasksList.SelectedIndex);
            Singlton.tasks[tasksList.SelectedIndex].vars = vars.ToArray();
            Singlton.tasks[tasksList.SelectedIndex].task = E1TText.Text;
            Singlton.tasks[tasksList.SelectedIndex].math = E1TFormula.Text;
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
            Edit1Grid.Visibility = Visibility.Hidden;
            EditSomeGrid.Visibility = Visibility.Hidden;
            tasksList.Focus();
        }
        private void ESTEnd(object sender, RoutedEventArgs e)
        {
            var tsk = new List<string>();
            var ans = new List<string>();
            foreach (object i in ESTTasks.Items)
            {
                if ((i is VisualTasks) && ((i as VisualTasks).Text != ""))
                {
                    tsk.Add((i as VisualTasks).Text);
                    ans.Add((i as VisualTasks).Value);
                }
            }
            Singlton.tasks[tasksList.SelectedIndex].Tasks = tsk;
            Singlton.tasks[tasksList.SelectedIndex].Answ = ans;
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
            Edit1Grid.Visibility = Visibility.Hidden;
            EditSomeGrid.Visibility = Visibility.Hidden;
            tasksList.Focus();
        }

        private void E1TText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vrs = new Dictionary<string, string>();
            var openFlag = false;
            var lst = new List<string>();
            foreach (char i in E1TText.Text)
            {
                if (openFlag)
                {
                    if (i == '}')
                        openFlag = false;
                    else
                        lst[lst.Count - 1] += i;
                }
                else
                {
                    if (i == '{')
                    {
                        openFlag = true;
                        lst.Add("");
                    }
                }
            }
            lst.RemoveAll(s => s.Trim() == "");
            Debug.WriteLine(string.Join(" ", lst.ToArray()));
            foreach (var i in Singlton.tasks[tasksList.SelectedIndex].visualVars)
            {
                vrs[i.Name] = i.Value;
            }
            var vars = new List<VisualVars>();
            foreach (var i in lst)
            {
                var f = false;
                foreach (var ii in vars) if (i == ii.Name) f = true;
                if (f) continue;
                try
                {
                    vars.Add(new VisualVars() { Name = i, Value = vrs[i] });
                }
                catch
                {
                    vars.Add(new VisualVars() { Name = i });
                }
            }
            E1TVars.ItemsSource = vars;

        }

        private void tasksList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditItem(null, null);
            LostSave();
        }

        private void NewWithVars(object sender, RoutedEventArgs e)
        {
            Singlton.tasks.Add(new Task() { type = true });
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
            LostSave();
        }

        private void NewWithotVars(object sender, RoutedEventArgs e)
        {
            Singlton.tasks.Add(new Task() { type = false });
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
            LostSave();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (SaveFolder == "")
            {
                try
                {
                    var path = new Microsoft.Win32.SaveFileDialog();
                    path.ShowDialog();
                    SaveFolder = path.FileName;
                    SaveFile(path.FileName);
                }
                catch { }
            }
            else
            {
                SaveFile(SaveFolder);
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = new Microsoft.Win32.SaveFileDialog();
                path.ShowDialog();
                SaveFolder = path.FileName;
                SaveFile(path.FileName);
            }
            catch { }
        }

        private void SaveFile(string s)
        {
            SaveF = true;
            ProjName.Text = s.Split('\\').Last().Split('.')[0];
            File.WriteAllText(s, JsonConvert.SerializeObject(Singlton.tasks, Formatting.Indented));
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            Singlton.tasks = new List<Task>();
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
            SaveFolder = "";
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            var path = new Microsoft.Win32.OpenFileDialog();
            path.ShowDialog();
            SaveFolder = path.FileName;
            Singlton.tasks = JsonConvert.DeserializeObject<List<Task>>(File.ReadAllText(path.FileName));
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
            SaveF = true;
            ProjName.Text = path.FileName.Split('\\').Last().Split('.')[0];
        }

        private void close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void maximize(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void LostSave() 
        {
            SaveF = false;
            if (SaveFolder != null)
                ProjName.Text = SaveFolder.Split('\\').Last().Split('.')[0] + "*";
            else
                ProjName.Text = "Новый тест*";
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
                this.WindowState = WindowState.Normal;
        }
    }
}
