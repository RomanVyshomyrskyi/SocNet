using System;
using MongoDB.Driver;
using My_SocNet_Win.Classes.DB.MongoDB;

namespace My_SocNet_Win.Classes.User;

public class MongoUserRepository : IUserRepository
{

    private readonly IMongoCollection<Users> _usersCollection;

    public MongoUserRepository(MongoDbService mongoDbService)
    {
        _usersCollection = mongoDbService.GetDatabase().GetCollection<Users>("users");
    }

    public async Task<Users> GetUserByIdAsync(int id)
    {
        return await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Users> GetUserByUserNameAsync(string userName)
    {
            return await _usersCollection.Find(u => u.UserName == userName).FirstOrDefaultAsync();
    }

    public async Task<bool> ValidateUserCredentialsAsync(string userName, string password)
    {
        var user = await GetUserByUserNameAsync(userName);
        return user != null && user.Password == password;
    }

    public async Task CreateUserAsync(Users user)
        {
            try
            {
                await _usersCollection.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                // Логування помилки
                Console.WriteLine($"Error inserting user: {ex.Message}");
                throw;
            }
        }
        

    public async Task<IEnumerable<Users>> GetAllUsersAsync()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }

}
