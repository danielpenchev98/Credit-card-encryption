using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows;
using System.Xml.Serialization;

namespace CardEncryptionDecryptionService
{
    public class XMLSerDeser
    {

        /// <summary>
        /// Help function for serializing the object into XML format (only the public data members and the properties, also requires Constructor withput params)
        /// </summary>
        /// <param name="details"></param>
        /// <param name="filename">DB's full path</param>
        public static void Serialize(AccountSettings[] details,string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AccountSettings[]));
                using (TextWriter writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, details);
                }
            }
            catch(FileNotFoundException)
            {
                //MessageBox.Show("The system couldnt locate the database, please retry", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Help function for deserializing the object from XML format
        /// </summary>
        /// <param name="filename">DB's full path</param>
        /// <returns>All stored accounts in the DB</returns>
        public static AccountSettings[] Deserialize(string filename)
        {
            AccountSettings[] details = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AccountSettings[]));
                using (TextReader writer = new StreamReader(filename))
                {
                    details = (AccountSettings[])serializer.Deserialize(writer);
                }
            }
            catch (FileNotFoundException)
            {
               // MessageBox.Show("The system couldnt locate the database, please retry", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
            return details;
        }



    }
}