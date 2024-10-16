using System;
using My_SocNet_Win.Classes.DB.HANA;
using My_SocNet_Win.Classes.DB.MongoDB;
using My_SocNet_Win.Classes.DB.MSSQL;
using My_SocNet_Win.Classes.DB.Neo4J;
using My_SocNet_Win.Classes.DB.Redis;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win;

public static class DatabaseConfigurator
{
    // Configure the database services based on the database type
    public static void ConfigureDatabaseServices(IServiceCollection services, IConfiguration configuration, string databaseType)
        {
            switch (databaseType)
            {
                case "MongoDb":
                    var mongoConnectionString = configuration.GetConnectionString("MongoDb");
                    if (string.IsNullOrEmpty(mongoConnectionString))
                    {
                        throw new ArgumentNullException(nameof(mongoConnectionString), "MongoDb connection string cannot be null or empty.");
                    }
                    var mongoDbService = new MongoDbService(mongoConnectionString);
                    services.AddSingleton(mongoDbService);
                    services.AddSingleton<IUserRepository>(new MongoUserRepository(mongoDbService));
                    break;
                case "Redis":
                    var redisConnectionString = configuration.GetConnectionString("Redis");
                    if (string.IsNullOrEmpty(redisConnectionString))
                    {
                        throw new ArgumentNullException(nameof(redisConnectionString), "Redis connection string cannot be null or empty.");
                    }
                    var redisService = new RedisService(redisConnectionString);
                    services.AddSingleton(redisService);
                    services.AddSingleton<IUserRepository>(new RedisUserRepository(redisService));
                    break;
                case "HANA":
                    var hanaConnectionString = configuration.GetConnectionString("HANA");
                    if (string.IsNullOrEmpty(hanaConnectionString))
                    {
                        throw new ArgumentNullException(nameof(hanaConnectionString), "HANA connection string cannot be null or empty.");
                    }
                    var hanaService = new HanaService(hanaConnectionString);
                    services.AddSingleton(hanaService);
                    services.AddSingleton<IUserRepository>(new HanaUserRepository(hanaService));
                    break;
                case "Neo4J":
                    var neo4jConnectionString = configuration.GetConnectionString("Neo4J");
                    if (string.IsNullOrEmpty(neo4jConnectionString))
                    {
                        throw new ArgumentNullException(nameof(neo4jConnectionString), "Neo4J connection string cannot be null or empty.");
                    }
                    var neo4jService = new Neo4jService(neo4jConnectionString);
                    services.AddSingleton(neo4jService);
                    services.AddSingleton<IUserRepository>(new Neo4jUserRepository(neo4jService));
                    break;
                case "MSSQL":
                    var mssqlConnectionString = configuration.GetConnectionString("MSSQL");
                    if (string.IsNullOrEmpty(mssqlConnectionString))
                    {
                        throw new ArgumentNullException(nameof(mssqlConnectionString), "MSSQL connection string cannot be null or empty.");
                    }
                    var mssqlService = new MssqlService(mssqlConnectionString);
                    services.AddSingleton(mssqlService);
                    services.AddSingleton<IUserRepository>(new MssqlUserRepository(mssqlService));
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
