using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.HttpUtilities.HttpModel
{
    public class SocketMessage
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public bool IsImportant { get; set; }
        public Type DataType { get; set; }
        public object Data { get; set; }
    }
}
