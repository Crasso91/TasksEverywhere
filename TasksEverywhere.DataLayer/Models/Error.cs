using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TasksEverywhere.DataLayer.Models
{
    public class Error
    {
        [Key]
        public long ErrorID { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public long CallID { get; set; }
        [JsonIgnore]
        public Call Call { get; internal set; }
    }
}