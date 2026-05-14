using MySql.Data.MySqlClient; // Ensure you installed MySql.Data via NuGet
using System;
using System.Text.RegularExpressions;
using System.Windows;
using static QRCoder.PayloadGenerator;

namespace SimpleAuthSystem
{
    public partial class SignupWindow : Window
    {

        private string connectionString =

           "server=localhost;database=museum_ticketing_system;uid=root;password=root;";

        public SignupWindow()
        {
            InitializeComponent();
        }

        private bool TextValidation()
        {
            errFirstName.Visibility = Visibility.Collapsed;
            errLastName.Visibility = Visibility.Collapsed;
            errEmail.Visibility = Visibility.Collapsed;
            errUser.Visibility = Visibility.Collapsed;
            errPass.Visibility = Visibility.Collapsed;
            errConfirmPass.Visibility = Visibility.Collapsed;

            bool hasError = false;
            // Validate First Name'
            string name = ValidateName(txtFirstName.Text);
            if (name != "valid")
            {
                errFirstName.Text = name;
                errFirstName.Visibility = Visibility.Visible;
                hasError = true;
               
            }
            // Validate First Name'
         
            name = ValidateName(txtLastName.Text);
            if (name != "valid")
            {
                errLastName.Text = name;
                errLastName.Visibility = Visibility.Visible;
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

           if (!ValidateEmail(txtEmail.Text))
            {
                errEmail.Visibility = Visibility.Visible;
                hasError = true;
                
            }
            if (!ValidatePassword(txtPass.Password))
            {
                errPass.Visibility = Visibility.Visible;

                hasError = true;
                
            }
            if (!ValidateConfirmPassword())
            {
                errConfirmPass.Visibility = Visibility.Visible;
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
        private readonly Regex EmailRegex = new Regex(
        @"^[A-Za-z][A-Za-z0-9]*@[A-Za-z]+\.[A-Za-z]+$",
        RegexOptions.Compiled
        );

        public string ValidateName(string name)
        {
            // 1. Check if blank
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Name is required.";
            }
          

            // 2. Check for numbers
            if (Regex.IsMatch(name, @"\d"))
            {
                return "Name must not contain numbers.";
            }

            // 3. Check for invalid special characters
            // Allowed: letters, space, period, dash
            if (!Regex.IsMatch(name, @"^[a-zA-Z .-]+$"))
            {
                return "Name can only contain letters, spaces, periods, and dashes.";
            }

            // 4. If all checks pass
            return "valid";
        }
        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errEmail.Text = "Email cannot be empty.";
                return false;
            }
            else if (!Regex.IsMatch(email, @"^[A-Za-z]"))
            {
                errEmail.Text="Email must start with a letter.";
                return false;
            }
            else if (!Regex.IsMatch(email, @"@"))
            {
                errEmail.Text = "Email must contain an '@' sign.";
                return false;
            }
            else if (Regex.Matches(email, "@").Count > 1)
            {
                errEmail.Text = "Email must not contain more than one '@' sign.";
                return false;
            }
            else if (!Regex.IsMatch(email, @"\."))
            {
                errEmail.Text = "Email must contain a period '.'.";
                return false;
            }
            else if (Regex.Matches(email, @"\.").Count > 1)
            {
                errEmail.Text = "Email must not contain more than one period '.'.";
                return false;
            }
            else
            {
                errEmail.Text = "Email is valid.";
                return true;
            }

        }
        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // CHECK USERNAME FIRST
                    string checkQuery = "SELECT COUNT(*) FROM staff WHERE username = @username";

                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", txtUser.Text.Trim());

                        int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (userExists > 0)
                        {
                            MessageBox.Show(
                                "Username already exists.",
                                "Duplicate Username",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                            return;
                        }
                    }

                    // CHECK EMAIL
                    if (DatabaseManager.EmailExists(txtEmail.Text.Trim()))
                    {
                        errEmail.Text = "Email already exists.";
                        errEmail.Visibility = Visibility.Visible;
                        return;
                    }

                    // VALIDATE INPUTS
                    if (!TextValidation())
                    {
                        MessageBox.Show(
                            "Invalid input!",
                            "Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    // INSERT USER
                    string insertQuery = @"INSERT INTO staff 
            (username, first_name, last_name, email, phone, password)
            VALUES
            (@username, @first_name, @last_name, @email, @phone, @password)";

                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@last_name", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", DatabaseManager.HashPassword(txtPass.Password));

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            txtUser.Clear();
                            txtFirstName.Clear();
                            txtLastName.Clear();
                            txtEmail.Clear();
                            txtPhone.Clear();
                            txtPass.Password = "";

                            MessageBox.Show(
                                "User Registered!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Registration failed.");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 0)
                {
                    MessageBox.Show(
                        "Cannot connect to server.\nCheck your internet or MySQL server.",
                        "Network Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(
                        "Database Error:\n" + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Signup Error: " + ex.Message);
            }
        }

        //private void btnSignup_Click(object sender, RoutedEventArgs e)
        //{

        //    if (!TextValidation())
        //    {
        //        MessageBox.Show("Invalid!");
        //        return;
        //    }
        //    MessageBox.Show("Valid!");

        //    if (DatabaseManager.EmailExists(txtEmail.Text))
        //    {
        //        errEmail.Text = "Email already exists.";
        //        errEmail.Visibility = Visibility.Visible;
        //        return;
        //    }

        //    try
        //    {
        //        using (MySqlConnection conn = new MySqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            MessageBox.Show("Connected!");

        //            // Check if username already exists
        //            string checkQuery = "SELECT COUNT(*) FROM staff WHERE username = @username";

        //            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
        //            {
        //                checkCmd.Parameters.AddWithValue("@username", txtUser.Text);

        //                int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());

        //                if (userExists > 0)
        //                {
        //                    MessageBox.Show("Username Already Exist ? ");
        //                    return;
        //                }
        //            }

        //            // Insert new user
        //            string insertQuery = @"INSERT INTO staff 
        //            (username, first_name, last_name, email, phone, password)
        //            VALUES
        //            (@username, @first_name, @last_name, @email, @phone, @password)";

        //            using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@username", txtUser.Text);
        //                cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text);
        //                cmd.Parameters.AddWithValue("@last_name", txtLastName.Text);
        //                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
        //                cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
        //                cmd.Parameters.AddWithValue("@password", txtPass.Password);

        //                int result = cmd.ExecuteNonQuery();

        //                if (result > 0)
        //                {
        //                    txtUser.Clear();
        //                    txtFirstName.Clear();
        //                    txtLastName.Clear();
        //                    txtEmail.Clear();
        //                    txtPhone.Clear();
        //                    txtPass.Password = "";

        //                    MessageBox.Show("User Registered!");
        //                    this.Close();
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Registration failed.");
        //                }
        //            }
        //        }
        //    }
        //    catch (MySqlException ex)
        //    {
        //        if (ex.Number == 0)
        //        {
        //            MessageBox.Show(
        //                "Cannot connect to server.\nCheck your internet or MySQL server.",
        //                "Network Error",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Error);
        //        }
        //        else
        //        {
        //            MessageBox.Show(
        //                "Database Error:\n" + ex.Message,
        //                "Error",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Signup Error: " + ex.Message);
        //    }
        //}


        public bool ValidatePassword(string password)
        {
            bool hasLetter = Regex.IsMatch(password, "[a-zA-Z]");
            bool hasNumber = Regex.IsMatch(password, "[0-9]");
            bool hasSpecial = Regex.IsMatch(password, "[^a-zA-Z0-9]");

            if (string.IsNullOrWhiteSpace(password))
            {
                errPass.Text = "Password is required.";
                return false;
            }
            else if(password.Length < 8)
            {
                errPass.Text = "Must be atleast 8.";
                return false;
            }
            else if (!Regex.IsMatch(password, "[a-zA-Z]"))
            {
                errPass.Text = "Password must contain at least one letter.";
                return false;
            }
            else if (!Regex.IsMatch(password, "[\\d]"))
            {
                errPass.Text = "Password must contain at least one number.";
                return false;
            }
            else if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                errPass.Text = "Password must contain at least one special character.";
                return false;
            }
            else if (txtPassReveal.Text != txtConfirmPassReveal.Text)
            {
                errPass.Text = "Passwords do not match.";
                return false;
            }

            // If all conditions are met
            else
            {
                errPass.Text = "";

                return true;

            }
            
        }

        public bool ValidateConfirmPassword()
        {
            MessageBox.Show(txtConfirmPass.Password + "\n" + txtConfirmPass.Password);
            if (string.IsNullOrWhiteSpace(txtConfirmPassReveal.Text))
            {
                errConfirmPass.Text = "Password is required.";
                return false;
            }
            else if (txtConfirmPass.Password != txtConfirmPass.Password)
            {
                errConfirmPass.Text = "Passwords do not match.";
                return false;
            }
            else
            {
                errConfirmPass.Text = "";

                return true;
            }

        }
        public bool ConfirmValidatePassword(string password)
        {
            bool hasLetter = Regex.IsMatch(password, "[a-zA-Z]");
            bool hasNumber = Regex.IsMatch(password, "[0-9]");
            bool hasSpecial = Regex.IsMatch(password, "[^a-zA-Z0-9]");

            if (string.IsNullOrWhiteSpace(password))
            {
                errConfirmPass.Text = "Password is required.";
                return false;
            }
            else if (password.Length <8)
            {
                errConfirmPass.Text = "Must be atleast 8.";
                return false;
            }
            else if (!Regex.IsMatch(password, "[a-zA-Z]"))
            {
                errConfirmPass.Text = "Password must contain at least one letter.";
                return false;
            }
            else if (!Regex.IsMatch(password, "[0-9]"))
            {
                errConfirmPass.Text = "Password must contain at least one number.";
                return false;
            }
            else if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                errConfirmPass.Text = "Password must contain at least one special character.";
                return false;
            }

            // If all conditions are met
            else
            {
                errConfirmPass.Text = "";

                return true;

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
        private void btnConfirmRevealToggle_Click(object sender, RoutedEventArgs e)
        {
            if (btnConfirmRevealToggle.IsChecked == true)
            {
                // Show Password: Copy dots to plain text and swap visibility
                txtConfirmPassReveal.Text = txtConfirmPass.Password;
                txtConfirmPass.Visibility = Visibility.Collapsed;
                txtConfirmPassReveal.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide Password: Copy plain text back to dots and swap visibility
                txtConfirmPass.Password = txtConfirmPassReveal.Text;
                txtConfirmPassReveal.Visibility = Visibility.Collapsed;
                txtConfirmPass.Visibility = Visibility.Visible;
            }
        }
    }
}