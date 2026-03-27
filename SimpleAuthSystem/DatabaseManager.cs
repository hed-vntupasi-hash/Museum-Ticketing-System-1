using System;
using System.Diagnostics;

using MySql.Data.MySqlClient;
using BCrypt.Net;
using System.Windows;
using System.Windows.Shell;
using Org.BouncyCastle.Crypto;

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

        public static Tuple<int[], string[], decimal[]> GetTicketTypes()
        {
            using (MySqlConnection conn = GetConnection())
            {
                List<int> ids = new List<int>();
                List<string> ticketNames = new List<string>();
                List<decimal> prices = new List<decimal>();
                try
                {
                    conn.Open();
                    string query = "SELECT ticket_type_id AS id, type_name, price FROM ticket_types";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(Convert.ToInt32(reader["id"]));
                            ticketNames.Add(reader["type_name"].ToString());
                            prices.Add(Convert.ToDecimal(reader["price"].ToString())); 
                        }
                        return Tuple.Create(ids.ToArray(), ticketNames.ToArray(), prices.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return Tuple.Create(new int[0], new string[0], new decimal[0]);
            }
        }

        public static Tuple<int[], string[], DateOnly[], DateOnly[]> GetEvents()
        {
            using (MySqlConnection conn = GetConnection())
            {
                List<int> ids = new List<int>();
                List<string> ticketNames = new List<string>();
                List<DateOnly> startDates = new List<DateOnly>();
                List<DateOnly> endDates = new List<DateOnly>();
                try
                {
                    conn.Open();
                    string query = "SELECT event_id AS id, event_name, start_date, end_date FROM events";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(Convert.ToInt32(reader["id"]));
                            ticketNames.Add(reader["event_name"].ToString());

                            startDates.Add(DateOnly.FromDateTime(reader.GetDateTime("start_date")));
                            endDates.Add(DateOnly.FromDateTime(reader.GetDateTime("end_date")));
                        }
                        return Tuple.Create(ids.ToArray(), ticketNames.ToArray(), startDates.ToArray(), endDates.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return Tuple.Create(new int[0], new string[0], new DateOnly[0], new DateOnly[0]);
            }
        }

        public static bool PurchaseTicket(int ticketType_Id, int event_Id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO tickets (ticket_type_id, event_id) VALUES (@ticket_type_id, @event_id)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ticket_type_id", ticketType_Id);
                    cmd.Parameters.AddWithValue("@event_id", event_Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}