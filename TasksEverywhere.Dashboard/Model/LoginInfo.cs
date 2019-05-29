using ICeScheduler.DataLayer.Models;

namespace Models
{
    public class LoginInfo
    {
        public string SessionKey { get; set; }
        public Account Account { get; set; }
    }
}