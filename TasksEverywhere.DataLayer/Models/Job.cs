using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TasksEverywhere.DataLayer.Models
{
    public class Job
    {
        [Key]
        public long JobID { get; set; }
        public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
        public string Key { get; set; }
        public bool Executing { get; set; }
        public string Description { get; set; }
        public DateTime StartedAt { get; set; }

        /* parent */
        public long ServerID { get; set; }
        [JsonIgnore]
        public virtual Server Server { get; set; }

        public Call LastCall { get { return Calls?.OrderByDescending(x => x.EndedAt)?.ToList().FirstOrDefault(); } }
        public bool IsExecutingCorrectly
        {
            get
            {
                if (Calls.Count > 1)
                {
                    return DateTime.Now <= LastCall.NextStart.AddMinutes(1);
                }
                else return true;
            }
        }
    }
}