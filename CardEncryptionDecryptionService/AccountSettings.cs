using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardEncryptionDecryptionService
{
    public class AccountSettings
    {

        private string username;
        private string password;

        //types of access
        private string[] access;

        //card numbers which were encrypted
        private CardInfo[] cards;

        //the encrypted numbers of cards
        private EncryptedCardInfo[] encryptedCards;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (value != null)
                {
                    username = value;
                }
                else
                {
                    username = default(string);
                }
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (value != null)
                {
                    password = value;
                }
                else
                {
                    password = default(string);
                }
            }
        }
        public string[] Access
        {
            get
            {
                string[] temp = new string[access.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = access[i];
                }
                return temp;
            }
            set
            {
                if (value != null)
                {
                    access = new string[value.Length];
                    for (int i = 0; i < access.Length; i++)
                    {
                        access[i] = value[i];
                    }
                }
            }
        }

        public CardInfo[] Cards
        {
            get
            {
                CardInfo[] temp = new CardInfo[cards.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = new CardInfo(cards[i].CardNumber, cards[i].EncrTimes, cards[i].Push);
                }
                return temp;
            }
            set
            {
                if (value != null)
                {
                    cards = new CardInfo[value.Length];
                    for (int i = 0; i < cards.Length; i++)
                    {
                        cards[i] = new CardInfo(value[i].CardNumber, value[i].EncrTimes, value[i].Push);
                    }
                }
                else
                {
                    cards = new CardInfo[0];
                }
            }
        }
        public EncryptedCardInfo[] EncryptedCards
        {
            get
            {
                EncryptedCardInfo[] temp = new EncryptedCardInfo[encryptedCards.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = new EncryptedCardInfo(encryptedCards[i].EncryptedNumber, encryptedCards[i].InitialPush, encryptedCards[i].Keys, encryptedCards[i].EncryptionLevel);
                }
                return temp;
            }
            set
            {
                if (value != null)
                {
                    encryptedCards = new EncryptedCardInfo[value.Length];
                    for (int i = 0; i < encryptedCards.Length; i++)
                    {
                        encryptedCards[i] = new EncryptedCardInfo(value[i].EncryptedNumber, value[i].InitialPush, value[i].Keys, value[i].EncryptionLevel);
                    }
                }
                else
                {
                    encryptedCards = new EncryptedCardInfo[0];
                }
            }
        }
        public AccountSettings(string usrnm, string passwrd, CardInfo[] crdInf, EncryptedCardInfo[] encrCrd, string[] access)
        {
            Username = usrnm;
            Password = passwrd;
            Cards = crdInf;
            EncryptedCards = encrCrd;
            Access = access;

        }
        public AccountSettings() : this(default(string), default(string), new CardInfo[0], new EncryptedCardInfo[0], new string[0])
        {

        }
        public AccountSettings(AccountSettings acc) : this(acc.Username, acc.Password, acc.Cards, acc.EncryptedCards, acc.Access)
        {

        }

        /// <summary>
        /// In order to avoid using more memory and time 
        /// </summary>
        /// <param name="index"></param>
        public void IncreaseEncrptionTimes(int index)
        {
            cards[index].IncreaseEncrTimes();
        }
    }
}