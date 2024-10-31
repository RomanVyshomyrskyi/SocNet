using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using My_SocNet_Win.Classes.DB;
using My_SocNet_Win.Classes.DB.HANA;
using My_SocNet_Win.Classes.DB.MongoDB;
using My_SocNet_Win.Classes.DB.MSSQL;
using My_SocNet_Win.Classes.DB.Neo4J;
using My_SocNet_Win.Classes.DB.Redis;
using My_SocNet_Win.Classes.DB.Strategies;
using My_SocNet_Win.Classes.User;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace My_SocNet_Win;

public class DatabaseConfigurator
{
    // Configure the database services based on the database type
    /// <summary>
    /// Configures the database services based on the specified database type.
    /// </summary>
    /// <param name="services">The service collection to add the database services to.</param>
    /// <param name="configuration">The configuration object to retrieve connection strings from.</param>
    /// <param name="databaseType">The type of the database to configure (e.g., "MongoDB", "HANA", "Neo4J", "MSSQL").</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the connection string for the specified database type is null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when an unsupported database type is specified.
    /// </exception>
    public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, string databaseType)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        IDatabaseStrategy strategy = databaseType switch
        {
            "MongoDB" => new MongoDbStrategy(),
            "HANA" => new HanaStrategy(),
            "Neo4J" => new Neo4JStrategy(),
            "MSSQL" => new MssqlStrategy(),
            _ => throw new ArgumentException($"Unsupported database type: {databaseType}")
        };

        strategy.Configure(services, configuration);
    }

    public static void EnsureAdminUserExists(IServiceProvider services, string databaseType)
    {
        IDatabaseStrategy strategy = databaseType switch
        {
            "MongoDB" => new MongoDbStrategy(),
            "HANA" => new HanaStrategy(),
            "Neo4J" => new Neo4JStrategy(),
            "MSSQL" => new MssqlStrategy(),
            _ => throw new ArgumentException($"Unsupported database type: {databaseType}")
        };

        strategy.EnsureAdminUserExists(services);
    }

    // Update the connection string in the configuration
    public static void UpdateConnectionString(IConfiguration configuration, string key, string value)
    {
        var configRoot = (IConfigurationRoot)configuration;
        var configProvider = configRoot.Providers.First();
        configProvider.Set(key, value);
    }
}
