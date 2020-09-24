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
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Scripting.Runtime;

namespace TestCreator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool someSelect = false;
        public string SaveFolder = "";
        public bool SaveF = true;


        ScriptEngine engine = Python.CreateEngine();
        ScriptScope scope;
        public MainWindow()
        {
            scope = engine.CreateScope();
            engine.SetSearchPaths(new[] { "Python/lib" });
            engine.ExecuteFile("Python/json_processing.py", scope);
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
           
        }

        private void focusList(object sender, RoutedEventArgs e)
        {
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
            LostSave();
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
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (tasksList.SelectedIndex != -1)
            {
                LostSave();
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
                    if (ii.Value == null) ii.Value = "";
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
            LostSave();
            EditItem(null, null);
        }

        private void NewWithVars(object sender, RoutedEventArgs e)
        {
            LostSave();
            Singlton.tasks.Add(new Task() { type = true });
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
        }

        private void NewWithotVars(object sender, RoutedEventArgs e)
        {
            LostSave();
            Singlton.tasks.Add(new Task() { type = false });
            tasksList.ItemsSource = "";
            tasksList.ItemsSource = Singlton.tasks;
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
            File.WriteAllText(s, JsonConvert.SerializeObject(Singlton.tasks, Formatting.Indented), Encoding.UTF8);
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
            Singlton.history = new List<List<Task>>();
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
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else 
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
            //this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
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
            var a = new List<Task>();
            foreach (var i in Singlton.tasks)
                a.Add((Task)i.Clone());
            Singlton.history.Add(a);
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


        bool ControlPress = false;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteItem(null, null);
            if (e.Key == Key.S && ControlPress)
            {
                if (SaveFolder != "" && SaveFolder != null)
                    SaveFile(SaveFolder);
                else
                {
                    var path = new Microsoft.Win32.SaveFileDialog();
                    path.ShowDialog();
                    SaveFolder = path.FileName;
                    SaveFile(path.FileName);
                }
            }
            if (e.Key == Key.Z && ControlPress)
            {
                Singlton.tasks = Singlton.history.Last();
                Singlton.history.Remove(Singlton.history.Last());
                tasksList.ItemsSource = Singlton.tasks;
            }
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
                ControlPress = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
                ControlPress = false;
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic function = scope.GetVariable("main_func");
                dynamic result = function(JsonConvert.SerializeObject(Singlton.tasks, Formatting.Indented), int.Parse(VarAmoInp.Text));
                result = System.Text.RegularExpressions.Regex.Unescape(result);

                //result = File.ReadAllText("F:\\txt.txt");
                List<List<rezultedTask>> rezultedTasks = JsonConvert.DeserializeObject<List<List<rezultedTask>>>(result);
                result = "";

                int xcCounter = 0;
                //foreach (var variant in rezultedTasks)
                for (int v = 1; v <= rezultedTasks.Count; v++)
                {
                    xcCounter += 1;
                    result += $"Вариант {v}\n";
                    for (int n = 1; n <= rezultedTasks[v - 1].Count; n++)
                    {
                        result += n + ". ";
                        result += rezultedTasks[v - 1][n - 1].task;
                        result += "\n";
                    }
                    if ((xcCounter == xcSelect.SelectedIndex + 1) || (v == rezultedTasks.Count))
                    {
                        xcCounter = 0;
                        result += "\xc";
                    }
                    else
                    {
                        result += "\n";
                    }
                }
                result += "Ответы\n";
                for (int v = 1; v <= rezultedTasks.Count; v++)
                {
                    result += $"Вариант {v}\n";
                    for (int n = 1; n <= rezultedTasks[v - 1].Count; n++)
                    {
                        result += n + ". ";
                        result += rezultedTasks[v - 1][n - 1].answer;
                        result += "\n";
                    }
                }

                var app = new Word.Application();
                var doc = app.Documents.Add();
                var r = doc.Range();
                r.Text = result;
                app.Visible = true;
                app.GoForward();
                app.PutFocusInMailHeader();
                //minimize(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void ExportTxt(object sender, RoutedEventArgs e)
        {
                dynamic function = scope.GetVariable("main_func");
                dynamic result = function(JsonConvert.SerializeObject(Singlton.tasks, Formatting.Indented), int.Parse(VarAmoInp.Text));
                result = System.Text.RegularExpressions.Regex.Unescape(result);

                //result = File.ReadAllText("F:\\txt.txt");
                List<List<rezultedTask>> rezultedTasks = JsonConvert.DeserializeObject<List<List<rezultedTask>>>(result);
                result = "";

                int xcCounter = 0;
                //foreach (var variant in rezultedTasks)
                for (int v = 1; v <= rezultedTasks.Count; v++)
                {
                    xcCounter += 1;
                    result += $"Вариант {v}\n";
                    for (int n = 1; n <= rezultedTasks[v - 1].Count; n++)
                    {
                        result += n + ". ";
                        result += rezultedTasks[v - 1][n - 1].task;
                        result += "\n";
                    }
                    if ((xcCounter == xcSelect.SelectedIndex + 1) || (v == rezultedTasks.Count))
                    {
                        xcCounter = 0;
                        result += "\xc";
                    }
                    else
                    {
                        result += "\n";
                    }
                }
                result += "Ответы\n";
                for (int v = 1; v <= rezultedTasks.Count; v++)
                {
                    result += $"Вариант {v}\n";
                    for (int n = 1; n <= rezultedTasks[v - 1].Count; n++)
                    {
                        result += n + ". ";
                        result += rezultedTasks[v - 1][n - 1].answer;
                        result += "\n";
                    }
                }

                var path = System.Environment.GetEnvironmentVariable("TEMP") + "\\\\" + DateTime.Now.ToString().Replace(':', '.') + ".txt";
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.Write(result);
                }
                Process process = new Process();
                process.StartInfo.FileName = "notepad.exe";
                process.StartInfo.Arguments = '\"' + path + '\"';
                process.Start();

        }
    }
}
