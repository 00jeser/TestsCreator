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
        List<Task> lst = JsonConvert.DeserializeObject<List<Task>>(File.ReadAllText("F:\\txt.json"));
        public MainWindow()
        {
            InitializeComponent();
            ScriptEngine engine = Python.CreateEngine();
            //var exePath = AppDomain.CurrentDomain.BaseDirectory;
            //engine.ExecuteFile(exePath + "\\Python\\py.py");
            //Console.Read();

            //List<Task> lst = new List<Task>();
            //lst.Add(new Task { task = "A = {a}, B = {b} Вычеслите A + B", type = true, vars = new Variable[] { new Variable { Name = "a", Range = "0-10" }, new Variable { Name = "b", lst = "0,-1,5".Split(',') } }, math = "Ответ = a + b" });
            //lst.Add(new Task { Tasks = new List<string>(new string[] { "A = 1, B = 0 Вычеслите A*B", "A = 0, B = 5 Вычеслите A*B" }), Answ = new List<string>(new string[] { "0", "0" }) });
            //File.WriteAllText("F:\\txt.json" ,JsonConvert.SerializeObject(lst, Formatting.Indented));
            tasksList.ItemsSource = lst;

        }
    }
}
