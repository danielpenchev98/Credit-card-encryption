using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardEncryptionDecryptionService
{
    public class EncryptedCardInfo
    {
        private string encryptedNumber;

        //used for the substitution encryption/decryption
        private int initialPush;
        
        //used for the transposition encryotion/decryption
        private int[] keys;

        //the number of current encryption of the card
        private int encryptionLevel;

        public string EncryptedNumber
        {
            get
            {
                return encryptedNumber;
            }
            set
            {
                if (value != null)
                {
                    encryptedNumber = value;
                }
                else
                {
                    encryptedNumber = default(string);
                }
            }
        }
        public int InitialPush
        {
            get
            {
                return initialPush;
            }
            set
            {
                if(value>=1)
                {
                    initialPush = value;
                }
                else
                {
                    initialPush = 1;
                }
            }
        }
        public int[] Keys
        {
            get
            {
                int[] temp = new int[keys.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = keys[i];
                }
                return temp;
            }
            set
            {
                if(value.Length>0)
                {
                    keys = new int[value.Length];
                    for(int i=0;i<keys.Length;i++)
                    {
                        keys[i] = value[i];
                    }
                }
            }
        }
        public int EncryptionLevel
        {
            get
            {
                return encryptionLevel;
            }
            set
            {
                if(value>0)
                {
                    encryptionLevel=value;
                }
                else
                {
                    encryptionLevel = -1;
                }
            }
        }
        public EncryptedCardInfo(string encrNum, int initialPush,int[] keys,int encryptionLevel)
        {
            EncryptedNumber = encrNum;
            InitialPush = initialPush;
            Keys = keys;
            EncryptionLevel = encryptionLevel;
        }
        public EncryptedCardInfo() : this(default(string),1,new int[0],-1)
        {

        }
    }
}