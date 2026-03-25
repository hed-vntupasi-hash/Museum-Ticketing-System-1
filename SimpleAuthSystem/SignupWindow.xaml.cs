using System;
using System.Windows;
using MySql.Data.MySqlClient; // Ensure you installed MySql.Data via NuGet

namespace SimpleAuthSystem
{
    public partial class SignupWindow : Window
    {
        public SignupWindow()
        {
            InitializeComponent();
        }

        private bool TextValidation()
        {
            errFirstName.Visibility = Visibility.Collapsed;
            errLastName.Visibility = Visibility.Collapsed;
            errUser.Visibility = Visibility.Collapsed;
            errPass.Visibility = Visibility.Collapsed;

            bool hasError = false;
            // Validate First Name
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                errFirstName.Visibility = Visibility.Visible;
                hasError = true;
            }

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                errLastName.Visibility = Visibility.Visible;
                hasError = true;
            }

            // Validate Username
            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                errUser.Visibility = Visibility.Visible;
                hasError = true;
            }

            string password = btnToggle.IsChecked == true ? txtPassReveal.Text : txtPass.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                errPass.Visibility = Visibility.Visible;
                hasError = true;
            }

            if (hasError)
            {
                return false;
            }
            return true;
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            if (TextValidation() == false)
            {
                return;
            }

            try
            {
                bool isSuccess = DatabaseManager.RegisterUser(txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtPhone.Text, txtUser.Text, txtPass.Password);

                if (isSuccess)
                {
                    MessageBox.Show("User Registered!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration Error: " + ex.Message);
            }
        }

        private void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            if (btnToggle.IsChecked == true)
            {
                // Show Password: Copy dots to plain text and swap visibility
                txtPassReveal.Text = txtPass.Password;
                txtPass.Visibility = Visibility.Collapsed;
                txtPassReveal.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide Password: Copy plain text back to dots and swap visibility
                txtPass.Password = txtPassReveal.Text;
                txtPassReveal.Visibility = Visibility.Collapsed;
                txtPass.Visibility = Visibility.Visible;
            }
        }
    }
}