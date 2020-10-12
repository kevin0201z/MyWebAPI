using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class LoginInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class sToken
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
}
