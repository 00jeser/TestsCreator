using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<string> Answ{ get { return new List<string>(answ); } set { answ = value.ToArray(); } }

        [JsonProperty("task")]
        public string task;

        [JsonProperty("variables")]
        public Variable[] vars;
    }

    public class Variable
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("range")]
        public string Range;

        [JsonProperty("choice")]
        public string[] lst;
    }
}
