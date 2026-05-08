using MySql.Data.MySqlClient; // Ensure you installed MySql.Data via NuGet
using System;
using System.Text.RegularExpressions;
using System.Windows;
using static QRCoder.PayloadGenerator;

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
            else if (!Regex.IsMatch(password, "[0-9]"))
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

                return false;

            }
            
        }

        public bool ValidateConfirmPassword()
        {

            if (string.IsNullOrWhiteSpace(txtConfirmPassReveal.Text))
            {
                errConfirmPass.Text = "Password is required.";
                return false;
            }
            else if (txtPassReveal.Text != txtConfirmPassReveal.Text)
            {
                errConfirmPass.Text = "Passwords do not match.";
                return false;
            }
            else
            {
                errConfirmPass.Text = "";

                return false;
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

                return false;

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