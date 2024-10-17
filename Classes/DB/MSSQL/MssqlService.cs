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
