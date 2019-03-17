using EncryptionDecryptionControls;
using InputControls;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.ServiceModel;

namespace CardsEncryptionDecryption
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        //string the id of the customer's session
        private string sessionId;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoginControl.loginOrRegister += OnRegistration;
            sessionId = null;
            lblSaveOption.Text = "Save in a file a table with your card numbers \n and their encrypted card numbers sorted by:";

        }

        /// <summary>
        /// if the user decides to close the Window then he should be logged off from the system in order not to cluster the list of logged in the system users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ServiceReference.LoginClient client = new ServiceReference.LoginClient();
            if (sessionId != null)
            {
                client.LogOff(sessionId);
            }
            e.Cancel = false;
        }

        /// <summary>
        /// Function for final checking of the user input 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool InputCheck(EventArgs args)
        {
            try
            {
                LoginInput input = (LoginInput)args;
                Regex username = new Regex(@"^[a-zA-Z][a-zA-Z0-9\-_\.]{7,25}$");
                Regex password = new Regex(input.Username);
                if (!username.IsMatch(input.Username))
                {
                    MessageBox.Show("The username should:\n- begin with a letter\n- should contain only a-z, A-Z, 0-9, '-', '_', '.'\n- its length should be 8-25 characters", "Username Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else if (password.IsMatch(input.Password))
                {
                    MessageBox.Show("The password cannot contain the username", "Password error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else if (input.Password.Length <= 7 || input.Password.Length >= 26)
                {
                    MessageBox.Show("The password should be between 8-25 characters", "Passwork error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Critical error. Please contact the developers", "System error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Function which  will use the Service for Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        [STAThread]
        public void OnLogin(object sender, EventArgs args)
        {
            ServiceReference.LoginClient client = new ServiceReference.LoginClient();

            if (!(args is RegisterInput))
            {
                bool validFormat = InputCheck(args);
                string doesExist = null;
                try
                {
                    //Checking if account exists in the system, by calling the Login web service 
                    doesExist = client.LogIn(((LoginInput)args).Username, ((LoginInput)args).Password);
                }
                catch (CommunicationException)
                {
                    MessageBox.Show("Ops something went wrong with the server, please try again", "Communication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (validFormat && doesExist != null)
                {
                    sessionId = doesExist;
                    //Hiding the logging form
                    LoginControl.Visibility = System.Windows.Visibility.Hidden;

                    //Showing the form for encryption and decryption
                    EncryptDecrypt.Visibility = System.Windows.Visibility.Visible;

                    //other stuff
                    lblSaveOption.Visibility = System.Windows.Visibility.Visible;
                    SortedByCardNumberBtn.Visibility = System.Windows.Visibility.Visible;
                    SortedByEncryptedNumbersBtn.Visibility = System.Windows.Visibility.Visible;
                    txtEcryptionResult.Visibility = System.Windows.Visibility.Visible;
                    txtDecryptionResult.Visibility = System.Windows.Visibility.Visible;
                    LogoffBtn.Visibility = System.Windows.Visibility.Visible;
                }
                else if (doesExist == null)
                {
                    MessageBox.Show("The username or the password is wrong or doenst exist,\n please type a valid one.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            client.Close();

        }

        /// <summary>
        /// Function which will use the Service for Registration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        [STAThread]
        public void OnRegistration(object sender, EventArgs args)
        {
            ServiceReference.LoginClient client = new ServiceReference.LoginClient();
            if (args is RegisterInput)
            {

                bool validFormat = InputCheck(args);
                bool doesntExist = false;
                try
                {
                  doesntExist=client.Register(((RegisterInput)args).Username, ((RegisterInput)args).Password, ((RegisterInput)args).Access);
                }
                catch (CommunicationException)
                {
                    MessageBox.Show("Ops something went wrong with the server, please try again", "Communication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (validFormat && doesntExist)
                {
                    LoginControl.InitialLoggingScreen();
                }
                else if (!doesntExist)
                {
                    MessageBox.Show("The username is already used,\n please select another one.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            client.Close();
        }

        /// <summary>
        /// Function for validating the format of the card
        /// </summary>
        /// <param name="pNumber"></param>
        /// <returns></returns>
        bool CheckLuhn(string pNumber)
        {
            int nSum = 0;
            int nDigits = pNumber.Length;
            int nParity = (nDigits - 1) % 2;
            for (int i = nDigits; i > 0; i--)
            {
                int nDigit = pNumber[i - 1] - '0';

                if (nParity == i % 2)
                    nDigit = nDigit * 2;

                nSum += nDigit / 10;
                nSum += nDigit % 10;
            }
            return nSum % 10 == 0;
        }

        /// <summary>
        /// Function which will use the Encryption Service to encrypt the card number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        [STAThread]
        public void OnAction(object sender, EventArgs args)
        {
            CardInput number = (CardInput)args;
            Regex reg = new Regex(@"^[3456][0-9]{15}$");
            ServiceReference.EncryptDecryptClient client = new ServiceReference.EncryptDecryptClient();
            if (number.Number.Length == 16 && number.EncryptDecrypt == false)
            {
                string res = client.Decrypt(sessionId, number.Number);
                if (res == null)
                {
                    MessageBox.Show("The encrypted number hasnt been found in the system or dont have encryption access", "Decryption Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    txtDecryptionResult.Text = res;
                }
            }
            else if (!CheckLuhn(number.Number) || !reg.IsMatch(number.Number))
            {
                MessageBox.Show("Invalid format of Card or Card number.It should consist of only 16 digits", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string res = client.Encrypt(sessionId, number.Number);
                if (res == null)
                {
                    MessageBox.Show("Exceeded the number of encryptions per card or dont have decryption access", "Encryption Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    txtEcryptionResult.Text = res;
                }

            }
            client.Close();

        }

        /// <summary>
        /// Logging out of the account
        /// </summary>
        private void Logoff()
        {
            ServiceReference.LoginClient client = new ServiceReference.LoginClient();
            client.LogOff(sessionId);
            sessionId = null;
            client.Close();

            //Hiding the additionaly added controls
            lblSaveOption.Visibility = System.Windows.Visibility.Hidden;
            SortedByCardNumberBtn.Visibility = System.Windows.Visibility.Hidden;
            SortedByEncryptedNumbersBtn.Visibility = System.Windows.Visibility.Hidden;
            txtDecryptionResult.Text = "";
            txtDecryptionResult.Visibility = System.Windows.Visibility.Hidden;
            txtEcryptionResult.Text = "";
            txtEcryptionResult.Visibility = System.Windows.Visibility.Hidden;
            LogoffBtn.Visibility = System.Windows.Visibility.Hidden;

            //Hiding the encryption/decryption form
            EncryptDecrypt.Clear();
            EncryptDecrypt.Visibility = System.Windows.Visibility.Hidden;

            //Showing the login form
            LoginControl.Visibility = System.Windows.Visibility.Visible;
            LoginControl.InitialLoggingScreen();
        }

        /// <summary>
        /// Event for Logging off from the the current account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogoffBtn_Click(object sender, RoutedEventArgs e)
        {
            Logoff();
        }

        /// <summary>
        /// Function for saving the table with cards and their encrypted numbers
        /// </summary>
        /// <param name="sender">used to distinquish the types of sorting</param>
        /// <param name="e"></param>
        private void SaveInfoInFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceReference.EncryptDecryptClient client = new ServiceReference.EncryptDecryptClient();
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = "Text Files(*.txt) | *.txt"
            };
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                if (((Button)sender).Content.ToString() == "By card numbers")
                {
                    client.CreateTextFileWithCardNumbersAndTheirEncryptedNumbers(sessionId, saveFile.FileName);
                }
                else
                {
                    client.CreateTextFileWithEncryptedNumbersAndTheirCardNumbers(sessionId, saveFile.FileName);
                }
            }
            else
            {
                MessageBox.Show("Couldnt find the file. Please try again", "I/O error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            client.Close();
        }
    }
}
