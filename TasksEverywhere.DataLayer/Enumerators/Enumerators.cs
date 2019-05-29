using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Enumerators
{
    public enum RoleType
    {
        Admin = 0,
        Client = 1,
        Inspector = 2,
        AdminAndInspector = Admin | Inspector
    }
}
