using System;
using StackExchange.Redis;

namespace My_SocNet_Win.Classes.DB.Redis;

public class RedisService : IDatabaseService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
    }

    public IDatabase GetDatabase()
    {
        return _database;
    }
    
    #region IDatabaseService implementation
    public void Connect()
    {
        // Connection is established in the constructor, so no additional implementation is needed here.
    }

    public void Disconnect()
    {
        _redis.Close();
    }

    public void EnsureDatabaseCreated()
    {
        throw new NotImplementedException();
    }
    #endregion
}
