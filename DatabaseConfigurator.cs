using System;
using MongoDB.Driver;
using My_SocNet_Win.Classes.DB.HANA;
using My_SocNet_Win.Classes.DB.MongoDB;
using My_SocNet_Win.Classes.DB.MSSQL;
using My_SocNet_Win.Classes.DB.Neo4J;
using My_SocNet_Win.Classes.DB.Redis;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win;

public class DatabaseConfigurator
{
    // Configure the database services based on the database type
    public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, string databaseType)
    {
        switch (databaseType)
        {
            case "MongoDB":
                var mongoConnectionString = configuration.GetConnectionString("MongoDB");
                var mongoClient = new MongoClient(mongoConnectionString);
                var mongoDatabase = mongoClient.GetDatabase("YourDatabaseName");
                services.AddSingleton(mongoDatabase);
                services.AddScoped<IUserRepository<MongoUsers>, MongoUserRepository>();
                break;
            case "Redis":
                var redisConnectionString = configuration.GetConnectionString("Redis");
                var redisService = new RedisService(redisConnectionString);
                services.AddSingleton(redisService);
                services.AddScoped<IUserRepository<RedisUsers>, RedisUserRepository>();
                break;
            case "HANA":
                var hanaConnectionString = configuration.GetConnectionString("HANA");
                var hanaService = new HanaService(hanaConnectionString);
                services.AddSingleton(hanaService);
                services.AddScoped<IUserRepository<HanaUsers>, HanaUserRepository>();
                break;
            case "Neo4J":
                var neo4jConnectionString = configuration.GetConnectionString("Neo4J");
                var neo4jService = new Neo4jService(neo4jConnectionString);
                services.AddSingleton(neo4jService);
                services.AddScoped<IUserRepository<Neo4jUsers>, Neo4jUserRepository>();
                break;
            case "MSSQL":
                var mssqlConnectionString = configuration.GetConnectionString("MSSQL");
                var mssqlService = new MssqlService(mssqlConnectionString);
                services.AddSingleton(mssqlService);
                services.AddScoped<IUserRepository<SqlUsers>, SqlUserRepository>();
                break;
            default:
                throw new Exception("Unsupported database type");
        }
    }

     public static void EnsureAdminUserExists(IServiceProvider services, string databaseType)
    {
        switch (databaseType)
        {
            case "MongoDB":
                var mongoUserRepository = services.GetService<IUserRepository<MongoUsers>>();
                if (mongoUserRepository != null)
                {
                    mongoUserRepository.EnsureAdminExistsAsync().Wait();
                }
                break;
            case "Redis":
                var redisUserRepository = services.GetService<IUserRepository<RedisUsers>>();
                if (redisUserRepository != null)
                {
                    redisUserRepository.EnsureAdminExistsAsync().Wait();
                }
                break;
            case "HANA":
                var hanaUserRepository = services.GetService<IUserRepository<HanaUsers>>();
                if (hanaUserRepository != null)
                {
                    hanaUserRepository.EnsureAdminExistsAsync().Wait();
                }
                break;
            case "Neo4J":
                var neo4jUserRepository = services.GetService<IUserRepository<Neo4jUsers>>();
                if (neo4jUserRepository != null)
                {
                    neo4jUserRepository.EnsureAdminExistsAsync().Wait();
                }
                break;
            case "MSSQL":
                var sqlUserRepository = services.GetService<IUserRepository<SqlUsers>>();
                if (sqlUserRepository != null)
                {
                    sqlUserRepository.EnsureAdminExistsAsync().Wait();
                }
                break;
            default:
                throw new Exception("Unsupported database type");
        }
    }
        // Update the connection string in the configuration
    public static void UpdateConnectionString(IConfiguration configuration, string key, string value)
    {
        var configRoot = (IConfigurationRoot)configuration;
        var configProvider = configRoot.Providers.First();
        configProvider.Set(key, value);
    }
}
