using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;

namespace TestCreator
{
    public class Task : ICloneable
    {
        public Task()
        {
            type = false;
            tasks = new string[0];
            answ = new string[0];
            task = "";
            vars = new Variable[0];
            math = "";

        }
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
        public List<VisualVars> visualVars
        {
            get
            {
                var ret = new List<VisualVars>();
                foreach (var i in vars)
                {
                    ret.Add(new VisualVars(i));
                }
                return ret;
            }
        }
        [JsonIgnore]
        public List<VisualTasks> visualTasks
        {
            get
            {
                var ret = new List<VisualTasks>();
                for (int i = 0; i < tasks.Length; i++)
                {
                    ret.Add(new VisualTasks() { Text = tasks[i], Value = answ[i] });
                }
                return ret;
            }
        }

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
                    return string.Join(";\n\\\n", tasks);
            }
        }

        public object Clone()
        {
            return new Task() { tasks = tasks, answ = answ, math = math, task = task + "", type = type, vars = vars };
        }
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
        public string Name { get; set; }
        public string Value { get; set; }
        public VisualVars(Variable v)
        {
            Name = v.Name;
            Value = v.Range != null ? v.Range : string.Join("; ", v.lst);
        }

        public VisualVars()
        {
        }
    }
    public class VisualTasks
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public VisualTasks()
        {
        }
    }
    public class rezultedTask 
    {
        public string task { get; set; }
        public string answer { get; set; }
    }
}
