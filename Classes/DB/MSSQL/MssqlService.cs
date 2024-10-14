using System;
using System.Data.SqlClient;

namespace My_SocNet_Win.Classes.DB.MSSQL;

public class MssqlService : IDatabaseService
{
    private readonly SqlConnection _connection;

    public MssqlService(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
    }

    public SqlConnection GetConnection()
    {
        return _connection;
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
}
