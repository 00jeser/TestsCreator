using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TestCreator.Properties;

namespace TestCreator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Settings.Default["Color1"] == null)
            {
                string colorizationValue = string.Format("{0:x}", Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", "00000000"));
                Color color = (Color)ColorConverter.ConvertFromString("#" + colorizationValue);
                color.A = 100;
                Settings.Default["Color1"] = new SolidColorBrush(color);
                color.A = 255;
                Settings.Default["Color2"] = new SolidColorBrush(color);
                Settings.Default.Save();
                float r = color.R, g = color.G, b = color.B;
                float s, v;
                Singlton.RGBtoHSV(r, g, b, out _, out s, out v);
                if (s >= 195 || v >= 195)
                {
                    Settings.Default.ColorF = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    Settings.Default.ColorF = new SolidColorBrush(Colors.White);
                }
            }
        }
    }
}
