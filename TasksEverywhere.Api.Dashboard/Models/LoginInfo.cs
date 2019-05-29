using TasksEverywhere.DataLayer.Models;

namespace TasksEverywhere.Api.Dashboard.Models
{
    public class LoginInfo
    {
        public string SessionKey { get; set; }
        public Account Account { get; set; }
    }
}