using CitizenFX.Core;
using CitizenFX.Core.Native;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace infCore.Server
{
    public class getData : BaseScript
    {
        public static string _connectionString;

        public static MySqlConnection getConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine("Connection string is not set.");
                throw new InvalidOperationException("Missing connection string.");
            }

            return new MySqlConnection(_connectionString);
        }


        public getData()
        {
            string hostName = API.GetConvar("infHostName", "localhost");
            string port = API.GetConvar("infPort", "3306");
            string userName = API.GetConvar("infUserName", "root");
            string password = API.GetConvar("infPassword", "");
            string database = API.GetConvar("infDatabase", "infCore");
            _connectionString = $"Server={hostName};Port={port};Database={database};User ID={userName};Password={password};";
            //Try to connect to database
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    //Create default tables
                    using( var createUserIdentifiers = new MySqlCommand(serverData.user_identifiers, connection))
                    {
                        createUserIdentifiers.ExecuteNonQuery();
                    }
                    using (var createUserData = new MySqlCommand(serverData.user_data, connection))
                    {
                        createUserData.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Error connecting to database: {ex.Message}");
                throw;
            }
        }
    }
}
