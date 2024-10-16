using System;

namespace My_SocNet_Win.Classes.User;

public class User : GetDBType
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public List<int> Friends { get; set; } = new List<int>();
    public DateTime DateOfCreation { get; set; }
    public DateTime LastLogin { get; set; }
    public List<string> Roles { get; set; } = new List<string>();

    private static IUserRepository GetUserRepository(IServiceProvider serviceProvider)
    {
        var databaseType = GetDatabaseType(serviceProvider);

        return databaseType switch
        {
            "MongoDb" => serviceProvider.GetRequiredService<MongoUserRepository>(),
            "Redis" => serviceProvider.GetRequiredService<RedisUserRepository>(),
            "HANA" => serviceProvider.GetRequiredService<HanaUserRepository>(),
            "Neo4J" => serviceProvider.GetRequiredService<Neo4jUserRepository>(),
            "MSSQL" => serviceProvider.GetRequiredService<MssqlUserRepository>(),
            _ => throw new Exception("Unsupported database type")
        };
    }

    public static async Task Insert(IEnumerable<User> users, IServiceProvider serviceProvider)
    {
        var databaseType = GetDatabaseType(serviceProvider);

        var userRepository = GetUserRepository(serviceProvider);

        foreach (var user in users)
        {
            await userRepository.CreateUserAsync(user);
        }
    }
    public static async Task Insert( User user, IServiceProvider serviceProvider)
    {
        var databaseType = GetDatabaseType(serviceProvider);

        var userRepository = GetUserRepository(serviceProvider);

        await userRepository.CreateUserAsync(user);
    }
}
