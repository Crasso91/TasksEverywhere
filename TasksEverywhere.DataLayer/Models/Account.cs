using TasksEverywhere.DataLayer.Enumerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Models
{
    public class Account
    {
        public long AccountID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public RoleType Roles { get; set; }
        [JsonIgnore]
        public virtual ICollection<Server> Servers { get; set; }

        public bool IsAdminOrIspector
        {
            get
            {
                return this.Roles == RoleType.Admin || this.Roles == RoleType.Inspector;
            }
        }

        public void ProceedIf(Func<Account, bool> _if, Action _canProceed, Action _else)
        {
            if (_if(this)) _canProceed();
            else _else();
        }
    }
}
