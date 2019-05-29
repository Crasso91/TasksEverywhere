using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TasksEverywhere.DataLayer.Models
{
    public class Call
    {
        [Key]
        public long CallID { get; set; }
        public virtual Error Error { get;  set; }
        public string FireInstenceID { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public DateTime NextStart { get;  set; }
        
        public long JobID { get; set; }
        [JsonIgnore]
        public virtual Job Job { get; set; }
    }
}