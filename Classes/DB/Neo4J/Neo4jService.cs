using System;
using Neo4j.Driver;

namespace My_SocNet_Win.Classes.DB.Neo4J;

public class Neo4jService : IDatabaseService
{
    private readonly IDriver _driver;

    public Neo4jService(string connectionString)
    {
        _driver = GraphDatabase.Driver(connectionString, AuthTokens.None);
    }
    
    public IDriver GetDriver()
    {
        return _driver;
    }


    #region IDatabaseService implementation
    public void Connect()
    {
        // Connection is established in the constructor, so no additional implementation is needed here.
    }

    public void Disconnect()
    {
        _driver.DisposeAsync().AsTask().Wait();
    }

    public void EnsureDatabaseCreated()
    {
        throw new NotImplementedException();
    }
    #endregion
}
