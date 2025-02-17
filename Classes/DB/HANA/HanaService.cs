using System;
using Sap.Data.Hana;

namespace My_SocNet_Win.Classes.DB.HANA;

public class HanaService : IDatabaseService
{
    private readonly HanaConnection _connection;

    public HanaService(string connectionString)
    {
        _connection = new HanaConnection(connectionString);
    }
    
     public HanaConnection GetConnection()
    {
        return _connection;
    }


    #region IDatabaseService implementation
    public void Connect()
    {
        _connection.Open();
    }

    public void Disconnect()
    {
        _connection.Close();
    }

    public void EnsureDatabaseCreated()
    {
        throw new NotImplementedException();
    }
    #endregion
}
