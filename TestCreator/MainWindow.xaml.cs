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

        Task draggingO;
        int draggingN;

        public MainWindow()
        {
            InitializeComponent();

            tasksList.ItemsSource = lst;
            try
            {
                string colorizationValue = string.Format("{0:x}", Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", "00000000"));
                Color color = (Color)ColorConverter.ConvertFromString("#" + colorizationValue);
                this.Resources["systemColor"] = new SolidColorBrush(color);
                float r = color.R, g = color.G, b = color.B, h = 0, s = 0, v = 0;
                Singlton.RGBtoHSV(r, g, b, out h, out s, out v);
                s /= 2;
                Singlton.HSVtoRGB(h, s, v, out r, out g, out b);
                color.R = (byte)r; color.G = (byte)g; color.B = (byte)b;
                this.Resources["lightSystemColor"] = new SolidColorBrush(color);
            }
            catch{}
        }
    }
}
