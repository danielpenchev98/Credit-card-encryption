using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDecryptionControls
{
    /// <summary>
    /// Event object 
    /// </summary>
    public class CardInput : EventArgs
    {
        public CardInput(string input,bool action)
        {
            Number = input;
            EncryptDecrypt = action;
        }

        /// <summary>
        /// Data member which the program uses to save the number of the card which will be encrypted or the encrypted number of the card which will be decrypted 
        /// </summary>
        private string number;
        
        /// <summary>
        /// Data member which will help as to distinquish the type of action : true - encrypt and false - decrypt
        /// </summary>
        bool encryptDecrypt;

        public bool EncryptDecrypt
        {
            get
            {
                return encryptDecrypt;
            }
            set
            {
                encryptDecrypt = value;
            }
        }
        public string Number
        {
            get { return number; }
            set
            {
                if (value != null)
                {
                    number = value;
                }
                else
                {
                    number = default(string);
                }
            }
        }

    }
}
