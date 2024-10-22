using System;
using System.Data.SqlClient;

namespace My_SocNet_Win.Classes.DB.MSSQL
{
    public class MssqlService : IDatabaseService
    {
        private readonly SqlConnection _connection;
        private readonly string _connectionString;

        public MssqlService(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString); // Initialize the connection
        }

        public SqlConnection GetConnection()
        {
            return _connection;
        }

        public void EnsureDatabaseCreated()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            var initialCatalog = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            try
            {
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        // Check if the database exists
                        command.CommandText = $@"
                        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{initialCatalog}')
                        BEGIN
                            CREATE DATABASE [{initialCatalog}];
                        END";
                        command.ExecuteNonQuery();
                    }
                }

                builder.InitialCatalog = initialCatalog;
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        // Create the necessary tables if they do not exist
                        command.CommandText = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                        CREATE TABLE Users (
                            Id INT PRIMARY KEY,
                            UserName NVARCHAR(50) NOT NULL,
                            PasswordHash NVARCHAR(255) NOT NULL,
                            Email NVARCHAR(50) NOT NULL,
                            DateOfCreation DATETIME NOT NULL,
                            LastLogin DATETIME NOT NULL,
                            ImgBinary NVARCHAR(MAX) NOT NULL
                        );

                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
                        CREATE TABLE UserRoles (
                            UserId INT NOT NULL,
                            Role NVARCHAR(50) NOT NULL,
                            FOREIGN KEY (UserId) REFERENCES Users(Id)
                        );

                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserFriends' AND xtype='U')
                        CREATE TABLE UserFriends (
                            UserId INT NOT NULL,
                            FriendId INT NOT NULL,
                            FOREIGN KEY (UserId) REFERENCES Users(Id),
                            FOREIGN KEY (FriendId) REFERENCES Users(Id)
                        );";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while ensuring database is created", e);
            }
        }

        public void Connect()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void Disconnect()
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public string ConnectionString => _connectionString;
    }
}