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
    //TODO: Implement the Connect and Disconnect methods
    public void Connect()
    {
        // Логіка підключення до Neo4J
    }

    public void Disconnect()
    {
        _driver.CloseAsync().Wait();
    }

}
