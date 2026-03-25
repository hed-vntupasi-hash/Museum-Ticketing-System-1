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
                    new MainWindow().Show();
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
    }
}