using System;
using My_SocNet_Win.Classes.DB.Redis;

namespace My_SocNet_Win.Classes.User;

public class RedisUserRepository : IUserRepository<RedisUsers>
{
    //TODO: Implement Redis-specific methods
    public Task AddUserAsync(RedisUsers user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task EnsureAdminExistsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RedisUsers>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<RedisUsers> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<RedisUsers> GetUserByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(RedisUsers user)
    {
        throw new NotImplementedException();
    }
}
