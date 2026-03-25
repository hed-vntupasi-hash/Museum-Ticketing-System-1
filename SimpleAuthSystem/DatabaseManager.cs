using System;
using System.Diagnostics;

using MySql.Data.MySqlClient;
using BCrypt.Net;
using System.Windows;

namespace SimpleAuthSystem
{
    public static class DatabaseManager
    {
        private static string connectionString = "server=localhost;user=root;database=museum_ticketing_system;port=3306;password=root;";


        // Internal helper to get the connection
        private static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // --- QUERY 1: Register User ---
        public static bool RegisterUser(string firstName, string lastName, string email, string phone, string username, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO staff (first_name, last_name, email, phone, username, password) VALUES (@first_name, @last_name, @email, @phone, @user, @pass)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@first_name", firstName);
                cmd.Parameters.AddWithValue("@last_name", lastName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", hashedPassword);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // --- QUERY 2: Validate Login ---
        public static bool ValidateLogin(string username, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            List<string> names = new List<string>();

            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT password FROM staff WHERE username=@user";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Access the column by name (or index)
                            names.Add(reader["password"].ToString());

                            //MessageBox.Show(hashedPassword + " " + names[0]);

                            if (BCrypt.Net.BCrypt.Verify(password, names[0]))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return false;
            }
        }
    }
}