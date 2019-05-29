using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Quartz.Context.Models
{
    public class ReflectionJobData
    {
        public string LibraryPath { get; set; }
        public string ClassName { get; set; }
        public string ConstructorArgs { get; set; }
        public string MethodName { get; set; }
        public string MethodArgs { get; set; }
    }
}
