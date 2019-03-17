using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputControls
{
    /// <summary>
    /// Used as an event object
    /// </summary>
    public class RegisterInput:LoginInput
    {
        /// <summary>
        /// Types of access
        /// </summary>
        private string[] access;

        /// <summary>
        /// the first element of bool[] access is encryptionAccess, the second is decryptionAccess
        /// </summary>
        public string[] Access
        {
            get
            {
                string[] temp = new string[access.Length];
                for(int i=0;i<temp.Length;i++)
                {
                    temp[i] = access[i];
                }
                return temp;
            }
            set
            {
                if (value != null&&value.Length!=0)
                {
                    access = new string[value.Length];
                    for(int i=0;i<value.Length;i++)
                    {
                        access[i] = value[i];
                    }
                }
                else
                {
                    access = new string[0];
                }
            }
        }
       
        public RegisterInput(string username,string password,string[] access):base(username,password)
        {
           Access = access;
        }
    }
}
