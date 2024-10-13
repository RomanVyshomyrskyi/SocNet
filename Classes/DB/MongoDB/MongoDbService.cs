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

    public IMongoDatabase GetDatabase()
    {

        return _database;

    }


    #region IDatabaseService implementation (usles for MongoDB)
       public void Connect()
    {
        // Connection is established in the constructor, so no additional implementation is needed here.
    }

    public void Disconnect()
    {
        // MongoDB .NET driver does not provide a direct method to disconnect.
        // Connections are managed by the driver and will be closed automatically when the application ends.
    }
    
    #endregion
}
