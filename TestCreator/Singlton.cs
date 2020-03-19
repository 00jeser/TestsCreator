using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TestCreator.Properties;

namespace TestCreator
{
    static class Singlton
    {
        static ScriptEngine engine = Python.CreateEngine();
        //var exePath = AppDomain.CurrentDomain.BaseDirectory;
        //engine.ExecuteFile(exePath + "\\Python\\py.py");
        //Console.Read();

        static public SolidColorBrush Color1 { get { return Settings.Default.Color1; } set { Settings.Default.Color1 = value; StyleChanged(Color1, Color2); Settings.Default.Save(); } }
        static public SolidColorBrush Color2 { get { return Settings.Default.Color2; } set { Settings.Default.Color2 = value; StyleChanged(Color1, Color2); Settings.Default.Save(); } }
        static public SolidColorBrush ColorF { get { return Settings.Default.ColorF; } set { Settings.Default.ColorF = value; FontColorChanged(value); Settings.Default.Save(); } }
        static public double SizeF { get { return Settings.Default.SizeF; } set { Settings.Default.SizeF = value; FontSizeChanged(value); Settings.Default.Save(); } }

        public delegate void StyleD(SolidColorBrush color1, SolidColorBrush color2);
        static public event StyleD StyleChanged;
        public delegate void SettingD(object v);
        static public event SettingD FontSizeChanged;
        static public event SettingD FontColorChanged;

        static public List<Task> tasks = new List<Task>();

        static void CreateJson()
        {
            List<Task> lst = new List<Task>();
            lst.Add(new Task { task = "A = {a}, B = {b} Вычеслите A + B", type = true, vars = new Variable[] { new Variable { Name = "a", Range = "0-10" }, new Variable { Name = "b", lst = "0,-1,5".Split(',') } }, math = "Ответ = a + b" });
            lst.Add(new Task { Tasks = new List<string>(new string[] { "A = 1, B = 0 Вычеслите A*B", "A = 0, B = 5 Вычеслите A*B" }), Answ = new List<string>(new string[] { "0", "0" }) });
            File.WriteAllText("F:\\txt.json", JsonConvert.SerializeObject(lst, Formatting.Indented));
        }

        public static void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v)
        {
            float min, max, delta;
            min = Math.Min(r, g);
            min = Math.Min(min, b);
            max = Math.Max(r, g);
            max = Math.Max(max, b);
            v = max; // v
            delta = max - min;
            if (max != 0)
                s = delta / max; // s
            else
            {
                // r = g = b = 0 // s = 0, v is undefined
                s = 0;
                h = -1;
                return;
            }
            if (r == max)
                h = (g - b) / delta; // between yellow & magenta
            else if (g == max)
                h = 2 + (b - r) / delta; // between cyan & yellow
            else
                h = 4 + (r - g) / delta; // between magenta & cyan
            h *= 60; // degrees
            if (h < 0)
                h += 360;
        }
        public static void HSVtoRGB(float h, float s, float v, out float r, out float g, out float b)
        {
            int i;
            float f, p, q, t;
            if (s == 0)
            {
                // achromatic (grey)
                r = g = b = v;
                return;
            }
            h /= 60;            // sector 0 to 5
            i = Convert.ToInt32(Math.Floor(h));
            f = h - i;          // factorial part of h
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));
            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:        // case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }
        }
    }
}
