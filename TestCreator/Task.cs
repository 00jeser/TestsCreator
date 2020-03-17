using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;

namespace TestCreator
{
    public class Task
    {
        [JsonProperty("vars")]
        public bool type;

        [JsonProperty("tasks")]
        private string[] tasks;
        [JsonIgnore]
        public List<string> Tasks { get { return new List<string>(tasks); } set { tasks = value.ToArray(); } }
        [JsonProperty("answers")]

        private string[] answ;
        [JsonIgnore]
        public List<string> Answ { get { return new List<string>(answ); } set { answ = value.ToArray(); } }

        [JsonProperty("task")]
        public string task;

        [JsonProperty("variables")]
        public Variable[] vars;

        [JsonIgnore]
        public List<VisualVars> visualVars{ get
            {
                var ret = new List<VisualVars>();
                foreach (var i in vars) 
                {
                    ret.Add(new VisualVars(i));
                }
                return ret;
            } }

        [JsonProperty("equation")]
        public string math;
        [JsonIgnore]
        public string Info
        {
            get
            {
                if (type)
                    return task + ";\n\n" + math;
                else
                    return string.Join(";\n\n", tasks);
            }
        }

        [JsonIgnore]
        public bool select 
        {
            set 
            {
                if (value)
                    selectBrush = new SolidColorBrush(new Color { A = 255, R = 230, G = 0, B = 255 });
                else
                    selectBrush = new SolidColorBrush(Colors.Transparent);
            }
        }
        [JsonIgnore]
        public SolidColorBrush selectBrush { get; set; }
    }

    public class Variable
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("range")]
        public string Range { get; set; }

        [JsonProperty("choice")]
        public string[] lst { get; set; }
    }
    public class VisualVars
    {
        public string Name{get;set;}
        public string Value { get; set; }
        public VisualVars(Variable v) 
        {
            Name = v.Name;
            Value = v.Range != null ? v.Range : string.Join("; ",v.lst);
        }
    }
}
