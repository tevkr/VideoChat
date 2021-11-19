using System;
using System.Data;
using System.IO;
using Npgsql;

namespace Server.Database
{
    public class PostgreService
    {
        private static readonly string _initFilePath = @"Database\init\init.sql";
        private static readonly string _host = "localhost";
        private static readonly string _user = "postgres";
        private static readonly string _password = "postgres";
        private static readonly string _database = "VideoChatDatabase";
        private static readonly string _port = "5432";
        private static readonly string _connectionString = $"User ID={_user};Password={_password};Host={_host};Port={_port};Database={_database};";
        public static (string, string) getUserIdAndUsername(string username)
        {
            string table = "users";
            string query = $"SELECT DISTINCT id, username FROM {table} WHERE username = '{username}';";
            string tableId = "", tableUsername = "";
            using (NpgsqlConnection connection = getConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    NpgsqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        tableId = dr[0].ToString();
                        tableUsername = dr[1].ToString();
                    }
                }
            }
            return (tableId, tableUsername);
        }
        public static void addUser(string id, string username, string password)
        {
            password = PasswordService.hashPassword(password);
            string table = "users";
            string query = $"INSERT INTO {table} VALUES ('{id}', '{username}', '{password}');";
            using (NpgsqlConnection connection = getConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        Console.WriteLine($"user {username} already exists.");
                    }
                }
            }
        }
        public static bool correctCredentials(string username, string password)
        {
            string table = "users";
            string query = $"SELECT DISTINCT username, password FROM {table} WHERE username = '{username}';";
            string tableUsername = "", tablePassword = "";
            using (NpgsqlConnection connection = getConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    NpgsqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        tableUsername = dr[0].ToString();
                        tablePassword = dr[1].ToString();
                    }
                }
            }
            return username == tableUsername && PasswordService.verifyHashedPassword(tablePassword, password);
        }
        public static bool userExists(string username)
        {
            string table = "users";
            string query = $"SELECT DISTINCT COUNT(*) FROM {table} WHERE username = '{username}';";
            Int64 count = 0;
            using (NpgsqlConnection connection = getConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    count = (Int64)command.ExecuteScalar();
                }
            }
            return count != 0;
        }
        public static void init()
        {
            string query;
            if (File.Exists(_initFilePath))
            {
                query = File.ReadAllText(_initFilePath);
            }
            else
            {
                Console.WriteLine($"Init file {Directory.GetCurrentDirectory()}\\{_initFilePath} doesn't exist.");
                return;
            }
            using (NpgsqlConnection connection = getConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Init file executed.");
                }
            }
        }
        public static bool testConnection()
        {
            using (NpgsqlConnection con = getConnection())
            {
                con.Open();
                return con.State == ConnectionState.Open;
            }
        }
        private static NpgsqlConnection getConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
