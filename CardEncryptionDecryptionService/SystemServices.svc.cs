using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace CardEncryptionDecryptionService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SystemServices : ILogin, IEncryptDecrypt
    {
        /// <summary>
        /// Name of the DataBase file
        /// </summary>
        // Because the service uses IIS Express the file should be there so we must use full path to the file if it is elsewhere
        private static readonly String filename = HostingEnvironment.ApplicationPhysicalPath+ "\\DB.xml";

        /// <summary>
        /// storing the logged user's accounts
        /// </summary>
        private static Dictionary<int, AccountSettings> logged = new Dictionary<int, AccountSettings>();

        #region Login
        /// <summary>
        /// Function for checking if there is a profile with the same username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>user's account</returns>
        public AccountSettings CheckIfValidAccount(string username, string password)
        {
            AccountSettings[] accounts = null;
            //Getting all the accounts from the DB
            try
            {
                accounts = XMLSerDeser.Deserialize(filename);
            }
            catch (FileNotFoundException)
            {
                Environment.Exit(Environment.ExitCode);
            }
            catch (DirectoryNotFoundException)
            {
                Environment.Exit(Environment.ExitCode);
            }

            //Checking if an account with those parameters exists(username,passwork)
            //And the program doesnt allow multiple devices to connect to the system with the same account(doesnt support the synchronization between multiple devices)
            if (accounts != null)
            {
                int found = -1;
                for (int i = 0; i < accounts.Length; i++)
                {
                    if (accounts[i].Username == username && accounts[i].Password == password)
                    {
                        found = i;
                    }
                }
                if (found >= 0)
                {
                    foreach (var item in logged)
                    {
                        if (item.Value.Username == username && item.Value.Password == password)
                        {
                            return null;
                        }
                    }
                    return accounts[found];
                }

            }
            return null;
        }

        /// <summary>
        /// Returns Id of the current session, in order not to upload the content of db every single time for that sigle account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>sessionId of the user</returns>
        public string LogIn(string username, string password)
        {
            AccountSettings found = CheckIfValidAccount(username, password);
            //If the user is in the system
            if (found != null)
            {
                int pos = logged.Count;
                logged[pos] = found;
                return pos.ToString();
            }
            return null;
        }

        /// <summary>
        /// Function for registering account with unique username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="access"></param>
        /// <returns>true - if the new account is created, false - if there exist an account with this username </returns>
        public bool Register(string username, string password, string[] access)
        {
            AccountSettings[] accounts = null;
            try
            {
                accounts = XMLSerDeser.Deserialize(filename);
            }
            catch (FileNotFoundException)
            {
                Environment.Exit(Environment.ExitCode);
            }
            catch(DirectoryNotFoundException)
            {
                Environment.Exit(Environment.ExitCode);
            }
            //Checking for already registered user with the same username
            if (accounts != null)
            {
                for (int i = 0; i < accounts.Length; i++)
                {
                    if (accounts[i].Username == username)
                    {
                        return false;
                    }
                }
            }

            //Could have done it with List<AccountSettings> but for some reason sometimes the Deserializer doesnt work with lists

            //Adding the new account the the system
            AccountSettings[] accountsUpdated = new AccountSettings[(accounts == null ? 0 : accounts.Length) + 1];
            for (int i = 0; i < accountsUpdated.Length - 1; i++)
            {
                accountsUpdated[i] = new AccountSettings(accounts[i]);
            }

            accountsUpdated[accountsUpdated.Length - 1] = new AccountSettings
            {
                Username = username,
                Password = password,
                Access = access
            };
            //Saving the newly updated version of the DB
            XMLSerDeser.Serialize(accountsUpdated, filename);
            return true;
        }

        /// <summary>
        /// Funtion for ending a session of a user
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>true, if the logoff is successful</returns>
        public bool LogOff(string sessionId)
        {
            //Checking if the sessionid is valid or not
            if (!Int32.TryParse(sessionId, out int pos) || !logged.ContainsKey(pos))
            {
                return false;
            }
            //Getting the DB information
            AccountSettings[] accounts = XMLSerDeser.Deserialize(filename);
            //Updating the DB information
            if (accounts != null)
            {
                for (int i = 0; i < accounts.Length; i++)
                {
                    if (accounts[i].Username == logged[pos].Username)
                    {
                        accounts[i].Cards = logged[pos].Cards;
                        accounts[i].EncryptedCards = logged[pos].EncryptedCards;
                    }
                }
            }
            //Saving the information in the DB
            XMLSerDeser.Serialize(accounts, filename);
            logged.Remove(pos);
            return true;
        }
        #endregion


        #region EncryptionDecryption

        /// <summary>
        /// Array of keys of type string
        /// </summary>
        //Next Version - to save the whole database of Keywords(1000 words maybe) in a file - for security reasons
        private static readonly string[] keyWords = { "zebras", "marvelous", "remember", "love", "pig", "wrestle", "uninterested", "hateful", "flap", "imaginary", "flippant", "force", "fold", "wrist", "reading", "quickest", "cakes", "rambunctious", "voyage", "lacking", "defeated", "highfalutin", "craven", "things", "receive", "shrug", "polite", "lopsided", "writing", "rural", "playground", "protective", "slave", "plate", "grease", "copper", "road", "warm", "snitch", "well-off", "jittery", "enchanted", "angle", "tremble", "agonizing", "awesome", "terrible", "island", "alert", "resonant", "piquant", "illustrious", "black-and-white", "legal", "poison", "knot", "tangible", "suffer", "windows", "welcome", "preach", "little", "scene", "second", "level", "mourn" };

        /// <summary>
        /// Implementation of the encrypting algorithm
        /// </summary>
        /// <param name="number"></param>
        /// <returns>encrypted number and its keyWordId</returns>
        private Tuple<string, int> CardEncryption(string number)
        {
            Random generator = new Random();
            //picking random word from the word list
            int luckyNumber = generator.Next(keyWords.Length);
            char[] key = keyWords[luckyNumber].ToCharArray();

            //Creating matrix for the transposition
            char[,] encryption = new char[1 + 16 / key.Length, key.Length];
            int counter = 0;
            for (int i = 0; i < encryption.GetLength(0); i++)
            {
                for (int j = 0; j < encryption.GetLength(1); j++)
                {
                    if (counter < number.Length)
                    {
                        encryption[i, j] = number[counter++];
                    }
                    else
                        encryption[i, j] = 'X';
                }
            }

            //sorting the columns
            string encrypted = "";
            for (int i = 0; i < encryption.GetLength(1); i++)
            {
                int pos = i;
                for (int j = i + 1; j < encryption.GetLength(1); j++)
                {
                    if (key[pos] > key[j])
                    {
                        pos = j;
                    }
                }
                char temp = '0';
                temp = key[pos];
                key[pos] = key[i];
                key[i] = temp;
                for (int j = 0; j < encryption.GetLength(0); j++)
                {
                    temp = encryption[j, pos];
                    encryption[j, pos] = encryption[j, i];
                    encryption[j, i] = temp;
                }
            }

            //getting the encrypted number
            for (int i = 0; i < encryption.GetLength(1); i++)
            {
                for (int j = 0; j < encryption.GetLength(0); j++)
                {
                    if (encryption[j, i] != 'X')
                        encrypted += encryption[j, i];
                }
            }
            //returning the number and the key's position
            return new Tuple<string, int>(encrypted, luckyNumber);
        }

        /// <summary>
        /// Function for encrypting the card number
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="cardNumber"></param>
        /// <returns>encrypted number</returns>
        public string Encrypt(string sessionId, string cardNumber)
        {
            if (Int32.TryParse(sessionId, out int id) && logged.ContainsKey(id))
            {
                //Checking if the user has access
                bool access = false;
                for (int i = 0; i < logged[id].Access.Length; i++)
                {
                    if (logged[id].Access[i] == "encryption")
                    {
                        access = true;
                    }
                }
                if (!access)
                {
                    return default(string);
                }

                //Getting the cards and the encrypted cards
                List<CardInfo> cards = logged[id].Cards.ToList();
                List<EncryptedCardInfo> encrCards = logged[id].EncryptedCards.ToList();
                int exist = -1;
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].CardNumber == cardNumber)
                    {
                        exist = i;
                        break;
                    }
                }
                if (exist == -1 || logged[id].Cards[exist].EncrTimes < 12)
                {
                    //Double Transposition
                    var encryptedNumber1 = CardEncryption(cardNumber);
                    int firstKey = encryptedNumber1.Item2;
                    var encryptedNumber2 = CardEncryption(encryptedNumber1.Item1);
                    int secondKey = encryptedNumber2.Item2;

                    Random rand = new Random();
                    //if the card doesnt exist then we wont have to use also substitution encryption
                    if (exist == -1)
                    {
                        int push = rand.Next(1, 15);
                        cards.Add(new CardInfo(cardNumber, 1, push));
                        encrCards.Add(new EncryptedCardInfo(encryptedNumber2.Item1, push, new int[] { firstKey, secondKey }, 1));
                        logged[id].Cards = cards.ToArray();
                        logged[id].EncryptedCards = encrCards.ToArray();
                        return encryptedNumber2.Item1;
                    }
                    else
                    {
                        //If the card has been already encrypted we will encrypt the card with random keys again but this time we will use substituion encryption on the result from the double transposition 
                        char[] encryption = encryptedNumber2.Item1.ToCharArray();
                        int push = logged[id].Cards[exist].Push;
                        int encrTimes = logged[id].Cards[exist].EncrTimes;
                        for (int i = 0; i < encryption.Length; i++)
                        {
                            encryption[i] = (char)((encryption[i] - '0' + (push + encrTimes - 1) % 16) % 10 + '0');
                        }
                        logged[id].IncreaseEncrptionTimes(exist);
                        string toReturn = new string(encryption);

                        encrCards.Add(new EncryptedCardInfo(toReturn, push, new int[] { firstKey, secondKey }, logged[id].Cards[exist].EncrTimes));
                        logged[id].EncryptedCards = encrCards.ToArray();
                        return toReturn;
                    }
                }
            }
            //If the card doesnt meet the requirements for encryption
            return default(string);
        }

        /// <summary>
        /// Help function for decryption
        /// </summary>
        /// <param name="toDecrypt">the encrypted number</param>
        /// <param name="kWord"> the position of the key in the list of keys</param>
        /// <returns>decrypted card number</returns>
        private string CardDecryption(char[] toDecrypt, int kWord)
        {
            char[] key = keyWords[kWord].ToCharArray();
            char[,] encryption = new char[1 + 16 / key.Length, key.Length];
            int lastRow = 16 % key.Length;
            int[] positions = new int[16];

            for (int i = 0; i < 16; i++)
            {
                positions[i] = i;
            }
            //getting the order of the colums before their sorting
            for (int i = 0; i < encryption.GetLength(1); i++)
            {
                int pos = i;
                for (int j = i + 1; j < encryption.GetLength(1); j++)
                {
                    if (key[pos] > key[j])
                    {
                        pos = j;
                    }
                }
                char temp = '0';
                temp = key[pos];
                key[pos] = key[i];
                key[i] = temp;

                int val = positions[i];
                positions[i] = positions[pos];
                positions[pos] = val;
            }

            //filling the matrix
            int counter = 0;
            for (int i = 0; i < encryption.GetLength(1); i++)
            {
                int column = positions[i];
                for (int j = 0; j < encryption.GetLength(0); j++)
                {
                    if (j == encryption.GetLength(0) - 1 && column > lastRow - 1)
                    {
                        encryption[j, column] = 'X';
                    }
                    else
                    {
                        encryption[j, column] = toDecrypt[counter++];
                    }
                }
            }

            //Getting the decrypted number
            counter = 0;
            char[] decryptedNumber = new char[16];
            for (int i = 0; i < encryption.GetLength(0); i++)
            {
                for (int j = 0; j < encryption.GetLength(1); j++)
                {
                    decryptedNumber[counter++] = encryption[i, j];
                    if (counter == 16)
                    {
                        return new string(decryptedNumber);
                    }
                }
            }
            return default(string);
        }

        /// <summary>
        /// Function for decrypting the encrypted card number
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="cardNumber"></param>
        /// <returns>card number</returns>
        public string Decrypt(string sessionId, string encrNumber)
        {
            if (Int32.TryParse(sessionId, out int id) && logged.ContainsKey(id))
            {

                //Checking if the user has access
                bool access = false;
                for (int i = 0; i < logged[id].Access.Length; i++)
                {
                    if (logged[id].Access[i] == "decryption")
                    {
                        access = true;
                    }
                }
                if (!access)
                {
                    return default(string);
                }

                //In order to decrypt the card the program needs to have it in the DB
                char[] toDecrypt = encrNumber.ToCharArray();
                EncryptedCardInfo[] encrCrd = logged[id].EncryptedCards;
                int pos = -1;
                for (int i = 0; i < encrCrd.Length; i++)
                {
                    if (encrCrd[i].EncryptedNumber == encrNumber)
                    {
                        pos = i;
                        break;
                    }
                }
                if (pos == -1)
                {
                    return default(string);
                }

                //if the card was encrypted before
                if (encrCrd[pos].EncryptionLevel >= 2)
                {
                    for (int i = 0; i < toDecrypt.Length; i++)
                    {
                        int move = toDecrypt[i] - '0' - (encrCrd[pos].InitialPush + encrCrd[pos].EncryptionLevel - 2) % 16;
                        toDecrypt[i] = (char)(move >= 0 ? move + '0' : (10 - Math.Abs(move) % 10) % 10 + '0');
                    }
                }

                string toReturn = CardDecryption(toDecrypt, encrCrd[pos].Keys[1]);
                toReturn = CardDecryption(toReturn.ToCharArray(), encrCrd[pos].Keys[0]);
                return toReturn;
            }
            return default(string);
        }

        /// <summary>
        /// Function for creating a file with ordered encrypted numbers and of cource their car numbers
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>true if the user's sessionId is legit</returns>
        public bool CreateTextFileWithEncryptedNumbersAndTheirCardNumbers(string sessionId, string filename)
        {
            if (Int32.TryParse(sessionId, out int id) && logged.ContainsKey(id))
            {
                //Creating a table which we will use it to sort the information about the card's number and the encrypted numbers and save it in a file
                List<Tuple<string, string>> table = new List<Tuple<string, string>>();
                List<EncryptedCardInfo> encr = logged[id].EncryptedCards.ToList();
                foreach (var item in encr)
                {
                    table.Add(new Tuple<string, string>(item.EncryptedNumber, Decrypt(sessionId, item.EncryptedNumber)));
                }

                //Sorting by the encrypted number
                var sortedByCardNumbers = table.OrderBy(item => item.Item1).Select(item => item);
                string[] content = new string[sortedByCardNumbers.Count()];
                int counter = 0;
                foreach (var item in sortedByCardNumbers)
                {
                    content[counter++] = "Encrypted number  :" + item.Item1 + " Enrypted numbers of this card are :" + item.Item2;
                }
                File.WriteAllLines(filename, content);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function for creating a file with ordered card numbers and  their encrypted numbers 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>true if the user's sessionId is legit</returns>
        public bool CreateTextFileWithCardNumbersAndTheirEncryptedNumbers(string sessionId, string filename)
        {
            if (Int32.TryParse(sessionId, out int id) && logged.ContainsKey(id))
            {
                List<Tuple<string, string>> table = new List<Tuple<string, string>>();
                List<EncryptedCardInfo> encr = logged[id].EncryptedCards.ToList();
                foreach (var item in encr)
                {
                    table.Add(new Tuple<string, string>(item.EncryptedNumber, Decrypt(sessionId, item.EncryptedNumber)));
                }

                //Sorting by the card number
                var sortedByCardNumbers = table.OrderBy(item => item.Item2).Select(item => item);
                string[] content = new string[sortedByCardNumbers.Count()];
                int counter = 0;
                foreach (var item in sortedByCardNumbers)
                {
                    content[counter++] = "Card number  :" + item.Item2 + " and its Enrypted number:" + item.Item1;
                }
                File.WriteAllLines(filename, content);
                return true;

            }

            return false;
        }
        #endregion

    }
}
