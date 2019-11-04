using System;
using System.Data.SqlClient;

namespace HotSpot.Modules
{
    public class Sql
    {
        private static readonly string AuthenicationTable = @"Server=localhost\SQLEXPRESS;Database=SmokeScreen;Trusted_Connection=true";

        public static bool AddAccount(string username, string password)
        {
            string sql = $"INSERT INTO Auth_Users (Username, Password) VALUES (@username, @password);";
            bool truth = false;

            using (SqlConnection authTable = new SqlConnection(AuthenicationTable))
            {
                using (SqlCommand command = new SqlCommand(sql, authTable))
                {
                    command.Connection.Open();
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    if (command.ExecuteNonQuery() != 0)
                    {
                        truth = true;
                    }
                }
            }
            return truth;
        }


        public static bool IsUsername(string username)
        {
            string sql = $"SELECT Count(*) FROM Auth_Users WHERE username = @username;";
            bool truth = false;

            using (SqlConnection authTable = new SqlConnection(AuthenicationTable))
            {
                using (SqlCommand command = new SqlCommand(sql, authTable))
                {
                    command.Connection.Open();
                    command.Parameters.AddWithValue("@username", username);
                    if ((int)command.ExecuteScalar() == 1)
                    {
                        truth = true;
                    }
                }
            }
            return truth;
        }

        public static bool IsPassword(string username, string password)
        {
            string sql = $"SELECT Count(*) FROM Auth_Users WHERE username = @username and password = @password;";
            bool truth = false;

            using (SqlConnection authTable = new SqlConnection(AuthenicationTable))
            {
                using (SqlCommand command = new SqlCommand(sql, authTable))
                {
                    command.Connection.Open();
                    command.Parameters.AddWithValue("@username", username);

                    command.Parameters.AddWithValue("@password", password);

                    if ((int)command.ExecuteScalar() == 1)
                    {
                        truth = true;
                    }
                }
            }
            return truth;
        }

    }
}
