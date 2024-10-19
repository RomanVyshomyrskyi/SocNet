using System;
using System.Data.SqlClient;

namespace My_SocNet_Win.Classes.DB.MSSQL;

public class MssqlService : IDatabaseService
{
    private readonly SqlConnection _connection;
    private readonly string _connectionString;

    public MssqlService(string connectionString)
    {
        try{
            _connection = new SqlConnection(connectionString);
        }
        catch (Exception e){
            throw new Exception("Error while creating connection", e);
        }
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public SqlConnection GetConnection()
    {
        return _connection;
    }

    public void EnsureDatabaseCreated()
    {
        // Implement logic to create database schema if it doesn't exist
        using (var command = _connection.CreateCommand())
        {
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
            _connection.Open();
            command.ExecuteNonQuery();
            _connection.Close();
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
