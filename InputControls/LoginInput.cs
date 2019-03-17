using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputControls
{
    public class LoginInput : EventArgs
    {
        private string username;

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
        
            }

        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
            }
        }
        public LoginInput(string username, string password)
        {
            Username = username;
            Password = password;
        }

    }
}
