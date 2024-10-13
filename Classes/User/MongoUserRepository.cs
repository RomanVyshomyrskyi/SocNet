using System;
using MongoDB.Driver;
using My_SocNet_Win.Classes.DB.MongoDB;

namespace My_SocNet_Win.Classes.User;

public class MongoUserRepository : IUserRepository
{

    private readonly IMongoCollection<User> _users;

    public MongoUserRepository(MongoDbService mongoDbService)
    {
        _users = mongoDbService.GetDatabase().GetCollection<User>("users");
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
        return await _users.Find(u => u.UserName == userName).FirstOrDefaultAsync();
    }

    public async Task<bool> ValidateUserCredentialsAsync(string userName, string password)
    {
        var user = await GetUserByUserNameAsync(userName);
        return user != null && user.Password == password;
    }

    public async Task CreateUserAsync(User user)
    {
        user.DateOfCreation = DateTime.UtcNow;
        await _users.InsertOneAsync(user);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

}
