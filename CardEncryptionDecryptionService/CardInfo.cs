using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardEncryptionDecryptionService
{
    public class CardInfo
    {
        private string cardNumber;
        
        /// <summary>
        /// substitution of a char with a char + push
        /// </summary>
        private int push;

        /// <summary>
        /// the number of the current encryption of the card
        /// </summary>
        private int encrTimes;
        public int EncrTimes
        {
            get
            {
                return encrTimes;
            }
            set
            {
                if(value>0)
                {
                    encrTimes = value;
                }
                else
                {
                    encrTimes = -1;
                }
            }
        }
        public string CardNumber
        {
            get
            {
                return cardNumber;
            }
            set
            {
                if (value != null)
                {
                    cardNumber = value;
                }
                else
                {
                    cardNumber = default(string);
                }
            }
        }
        public int Push
        {
            get
            {
                return push;
            }
            set
            {
                if (value > 0)
                {
                    push = value;
                }
                else
                {
                    push = 1;
                }
            }
        }
        public CardInfo(string crdNumber, int encryptedTimes, int push)
        {
            CardNumber = crdNumber;
            EncrTimes = encryptedTimes;
            Push=push;
        }

        public CardInfo() : this(default(string), 0, 1)
        {

        }

        public void IncreaseEncrTimes()
        {
            encrTimes++;
        }
    }
}