using System;
using System.Windows;
using System.Windows.Controls;

namespace InputControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public event EventHandler loginOrRegister;
               
        public LoginControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Used for validation of the username through ValidationRule
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Button for logging into the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            loginOrRegister?.Invoke(this, new LoginInput(txtUserName.Text, txtPassword.Password));
        }

        /// <summary>
        /// Link which swaps to registering form when it is clicked on it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkRegister_Click(object sender, RoutedEventArgs e)
        {

            txtPassword.Clear();
            txtUserName.Clear();
            LoginBtn.Visibility = System.Windows.Visibility.Hidden;
            linkRegister.Visibility = System.Windows.Visibility.Hidden;
            Title.Content = "Registeration";
            AccessBox.Visibility = System.Windows.Visibility.Visible;
            btnReset.Visibility = System.Windows.Visibility.Visible;
            btnSummit.Visibility = System.Windows.Visibility.Visible;
            btnCancel.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Button for registering new account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSummit_Click(object sender, RoutedEventArgs e)
        {
            string[] access=new string[2];
            if (EncrAccessCheckBox.IsChecked==true&&DecrAccessCheckBox.IsChecked==true)
            {
                access[0] = "encryption";
                access[1] = "decryption";
            }
            else if(EncrAccessCheckBox.IsChecked == true)
            {
                access = new string[1];
                access[0] = "encryption";
            }
            else if(DecrAccessCheckBox.IsChecked==true)
            {
                access = new string[1];
                access[0] = "decryption";
            }
            else
            {
                access = new string[0];
            }
            loginOrRegister?.Invoke(this,new RegisterInput(txtUserName.Text, txtPassword.Password,access));
        }
        /// <summary>
        /// Transition from Registratotion form to Login form
        /// </summary>
        public void InitialLoggingScreen()
        {
            txtPassword.Clear();
            txtUserName.Clear();
            LoginBtn.Visibility = System.Windows.Visibility.Visible;
            linkRegister.Visibility = System.Windows.Visibility.Visible;
            lblUserName.Visibility = System.Windows.Visibility.Visible;
            lblPass.Visibility = System.Windows.Visibility.Visible;
            Title.Content = "Login";
            AccessBox.Visibility = System.Windows.Visibility.Hidden;
            btnReset.Visibility = System.Windows.Visibility.Hidden;
            btnSummit.Visibility = System.Windows.Visibility.Hidden;
            btnCancel.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// clearing the register form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            txtPassword.Clear();
            txtUserName.Clear();
            EncrAccessCheckBox.IsChecked=false;
            DecrAccessCheckBox.IsChecked=false;
        }

        /// <summary>
        /// Returning back to logging form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            InitialLoggingScreen();
        }
    }
}
