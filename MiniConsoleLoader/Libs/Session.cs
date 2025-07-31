using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniConsoleLoader
{
    public class Session
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Subscription { get; set; }
        public bool IsLoggedIn { get; set; } = false;
    }
}
