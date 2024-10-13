using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using My_SocNet_Win.Classes.DB.Neo4J;
using Neo4j.Driver;

namespace My_SocNet_Win.Classes.User;

public class Neo4jUserRepository : IUserRepository
{
    private readonly IDriver _driver;

    public Neo4jUserRepository(Neo4jService neo4jService)
    {
        _driver = neo4jService.GetDriver();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        // Implementation for getting user by ID from Neo4j
        throw new NotImplementedException();
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
        // Implementation for getting user by UserName from Neo4j
        throw new NotImplementedException();
    }

    public async Task<bool> ValidateUserCredentialsAsync(string userName, string password)
    {
        var user = await GetUserByUserNameAsync(userName);
        return user != null && user.Password == password;
    }

    public async Task CreateUserAsync(User user)
    {
        user.DateOfCreation = DateTime.UtcNow;
        // Implementation for creating user in Neo4j
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        // Implementation for getting all users from Neo4j
        throw new NotImplementedException();
    }
}
