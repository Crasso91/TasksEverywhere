using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Models
{
    public class Server
    {
        [Key]
        public long ServerID { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string AppId { get; set; }
        public string AppToken { get; set; }
        [JsonIgnore]
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

        /* parent */
        public long AccountID {get;set;}
        [JsonIgnore]
        public virtual ICollection<Account> Accounts { get; set; }

        public bool IsExecutingCorrectly {
            get
            {
                var _result = true;
                foreach(var job in Jobs)
                {
                    if (_result) _result = job.IsExecutingCorrectly;
                    else break;
                }
                return _result;
            }
        }
    }
}
