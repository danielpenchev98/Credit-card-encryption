using System;
using System.Windows;
using System.Windows.Controls;

namespace EncryptionDecryptionControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EncrDecrControl : UserControl
    {
        public event EventHandler action;
        public EncrDecrControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Function for Clearing the textboxes of the current UserControl
        /// </summary>
        public void Clear()
        {
            txtCardNumber.Clear();
            txtEncryptedNumber.Clear();
        }

        /// <summary>
        /// Button for Encryption
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            action?.Invoke(this, new CardInput(txtCardNumber.Text, true));
        }

        /// <summary>
        /// Button for Decrypting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            action?.Invoke(this, new CardInput(txtEncryptedNumber.Text, false));
        }
    }
}
