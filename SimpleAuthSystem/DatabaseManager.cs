using System;
using System.Diagnostics;

using MySql.Data.MySqlClient;
using BCrypt.Net;
using System.Windows;
using System.Windows.Shell;
using Org.BouncyCastle.Crypto;
using System.Data;
using ZXing;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using System.Text.RegularExpressions;

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

        public static void SetTicketQrCode(string qrCode)
        {
            int id = -1;
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT LAST_INSERT_ID() AS id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            id = Convert.ToInt32(reader["id"]);
                            reader.Close();
                        }
                    }

                    query = "UPDATE tickets SET qrcode=@qrcode WHERE ticket_id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@qrCode", qrCode);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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

        public static string PurchaseTicket(int ticketType_Id, int event_Id, string qr)
        {
            //MessageBox.Show(qr, "Purchase Ticket");
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    //string query = "INSERT INTO tickets (ticket_type_id, event_id) VALUES (@ticket_type_id, @event_id)";
                    string query = "CALL PurchaseTicket(@ticket_type_id, @event_id, @qr)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ticket_type_id", ticketType_Id);
                    cmd.Parameters.AddWithValue("@event_id", event_Id);
                    cmd.Parameters.AddWithValue("@qr", qr);

                    //return cmd.ExecuteNonQuery() > 0;
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return "Success";
                    }
                    //return true;
                }
                catch (MySqlException ex)
                {
                    string columnName = "";
                    var match = Regex.Match(ex.Message, @"for column '([^']+)'");
                    if (ex.Number == 1366 && match.Success)
                    {
                        columnName = match.Groups[1].Value;
                        MessageBox.Show(columnName + "\n\n" + ex.Message, "Data Type Mismatch");
                        return "data type mismatch";
                    } else if (ex.Number == 1366)
                    {
                        return "TruncatedWrongValue";
                    }
                    else if (ex.Number == 1452)
                    {
                        match = Regex.Match(ex.Message, @"FOREIGN KEY \(`([^`]+)`\)");
                        columnName = match.Groups[1].Value;

                        if (columnName == "ticket_type_id")
                            return "NonExistentTicketTypeId";
                        else if (columnName == "event_id")
                            return "NonExistentTicketEventId";

                        MessageBox.Show("NonExistentTicketTypeId");

                        return "NonExistentForeignKey";

                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                    return ex.Number.ToString();



                }
            }
            return "error";
        }
        public static DataTable GetTickets()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query =
                        "SELECT T.ticket_id as id, E.event_name AS event, T2.type_name AS type, T2.price AS price " +
                        "FROM tickets AS T " +
                        "JOIN events AS E ON T.event_id = E.event_id " +
                        "JOIN ticket_types AS T2 ON T.ticket_type_id = T2.ticket_type_id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }


        public static Tuple<bool, string> TapTicket(string qrCode)
        {
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    //string query = "SELECT ticket_id AS id FROM tickets WHERE qrcode=@qrcode;";
                    string query = "CALL GetTicketValidity(@qrcode)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@qrcode", qrCode);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        //MessageBox.Show(qrCode + "\n" + Convert.ToBoolean(reader["@isValid"]).ToString());
                        return Tuple.Create
                        (
                            Convert.ToBoolean(reader["isValid"]),
                            reader["message"].ToString()
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //MessageBox.Show(qrCode + "\nTicket does not exist.");
                }
                return Tuple.Create(false, "Error processing ticket");
            }
        }


        public static bool CreateEvent(string name, string description, DateTime startDate, DateTime endDate)
        {
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO events (event_name, description, start_date, end_date) VALUES (@name, @description, @startDate, @endDate)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
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

        public static Tuple<string[], int[], decimal[]> GetTicketsByEvent()
        {
            List<string> events = new List<string>();
            List<int> ticketsSold = new List<int>();
            List<decimal> totalProfit = new List<decimal>();

            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "select t.event_id, e.event_name, count(*) as `tickets_sold`, sum(tt.price) as profit from events as e right join tickets as t on e.event_id = t.event_id left join ticket_types as tt on t.ticket_type_id = tt.ticket_type_id group by t.event_id;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        events.Add(reader["event_name"].ToString());
                        ticketsSold.Add(reader.GetInt32("tickets_sold"));
                        totalProfit.Add(reader.GetDecimal("profit"));
                    }
                    return Tuple.Create(events.ToArray(), ticketsSold.ToArray(), totalProfit.ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return Tuple.Create(new string[0], new int[0], new decimal[0]);
            }
        }
    }
}