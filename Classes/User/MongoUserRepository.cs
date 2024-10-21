using System;
using MongoDB.Driver;

namespace My_SocNet_Win.Classes.User;

public class MongoUserRepository : IUserRepository<MongoUsers>
{
    private readonly IMongoCollection<MongoUsers> _collection;

    public MongoUserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<MongoUsers>("Users");
    }

    public async Task<MongoUsers> GetUserByIdAsync(string id)
    {
        if (!int.TryParse(id, out int userId))
        {
            throw new ArgumentException("Invalid user ID format", nameof(id));
        }
        return await _collection.Find(u => u.Id == userId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MongoUsers>> GetAllUsersAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task AddUserAsync(MongoUsers user)
    {
        await _collection.InsertOneAsync(user);
    }

    public async Task UpdateUserAsync(MongoUsers user)
    {
        await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task DeleteUserAsync(string id)
    {
        if (!int.TryParse(id, out int userId))
        {
            throw new ArgumentException("Invalid user ID format", nameof(id));
        }
        await _collection.DeleteOneAsync(u => u.Id == userId);
    }

    public Task EnsureAdminExistsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MongoUsers> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        throw new NotImplementedException();
    }
}
