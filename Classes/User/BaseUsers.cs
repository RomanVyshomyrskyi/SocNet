using System;
using MongoDB.Libmongocrypt;

namespace My_SocNet_Win.Classes.User;

public abstract class BaseUsers : GetDBType
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public string Email { get; set; } = "";	
    public List<int> Friends { get; set; } = new List<int>();
    public DateTime DateOfCreation { get; set; }
    public DateTime LastLogin { get; set; }
    public List <string> Roles { get; set; } = new List<string>();
    public string ImgBinary { get; set; } = "";
}


public class MongoUsers : BaseUsers
{
    // MongoDB-specific properties or methods

    
}

public class RedisUsers : BaseUsers
{
    // Redis-specific properties or methods
}

public class HanaUsers : BaseUsers
{
    // HANA-specific properties or methods
}

public class Neo4jUsers : BaseUsers
{
    // Neo4J-specific properties or methods
}

public class SqlUsers : BaseUsers
{
    // MSSQL-specific properties or methods
}