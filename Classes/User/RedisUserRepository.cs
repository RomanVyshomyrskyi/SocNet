using System;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using My_SocNet_Win.Classes.DB.Redis;

namespace My_SocNet_Win.Classes.User;

public class RedisUserRepository : IUserRepository
{
     private readonly IDatabase _database;

    public RedisUserRepository(RedisService redisService)
    {
        _database = redisService.GetDatabase();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var userJson = await _database.StringGetAsync(id.ToString());
        return userJson.HasValue ? JsonConvert.DeserializeObject<User>(userJson) : null;
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
        // Implementation for getting user by UserName from Redis
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
        var userJson = JsonConvert.SerializeObject(user);
        await _database.StringSetAsync(user.Id.ToString(), userJson);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        // Implementation for getting all users from Redis
        throw new NotImplementedException();
    }
    
}
