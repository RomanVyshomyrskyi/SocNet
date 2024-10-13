using System;
using MongoDB.Driver;

namespace My_SocNet_Win.Classes.DB.MongoDB;

public class MongoDbService : IDatabaseService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(string connectionString){
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("My_SocNet_Win");
    }

    //TODO: Implement the Connect and Disconnect methods
    public void Connect()
    {
        throw new NotImplementedException();
    }

    public void Disconnect()
    {
        throw new NotImplementedException();
    }
}
