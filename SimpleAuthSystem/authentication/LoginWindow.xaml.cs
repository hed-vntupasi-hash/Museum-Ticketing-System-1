using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace SimpleAuthSystem
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isAuthenticated = DatabaseManager.ValidateLogin(txtUser.Text, txtPass.Password);

                if (isAuthenticated)
                {
                    new Menulist().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Error: " + ex.Message);
            }
        }

        private void btnGoToSignup_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow signup = new SignupWindow();
            signup.ShowDialog(); // Opens signup as a pop-up
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