using System;
using StackExchange.Redis;

namespace My_SocNet_Win.Classes.DB.Redis;

public class RedisService : IDatabaseService
{
    private readonly ConnectionMultiplexer _redis;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
    }
    
    //TODO: Implement the Connect and Disconnect methods
    public void Connect()
    {
        // Логіка підключення до Redis
    }

    public void Disconnect()
    {
        _redis.Close();
    }

}
