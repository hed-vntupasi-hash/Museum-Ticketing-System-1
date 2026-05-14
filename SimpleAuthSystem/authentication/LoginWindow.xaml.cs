using System;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using BCrypt.Net;
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
                    new Dashboard().Show();
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

    


        //public static bool ValidateLogin(string username, string password)
        //{
        //    string connectionString = "server=localhost;user=root;database=museum_ticketing_system;port=3306;password=root;Connection Timeout=5;";
        //    using (MySqlConnection conn = new MySqlConnection(connectionString))
        //    {
        //        conn.Open();

        //        string query = "SELECT Password FROM Users WHERE Username=@Username";

        //        MySqlCommand cmd = new MySqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@Username", username);

        //        object result = cmd.ExecuteScalar();

        //        if (result != null)
        //        {
        //            string storedHash = result.ToString();

        //            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        //        }
        //    }

        //    return false;
        //}


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