using System;

namespace My_SocNet_Win.Classes.User;

public class Neo4jUserRepository : IUserRepository<BaseUsers>
{
    //TODO: Implement Neo4j-specific methods
    public Task AddUserAsync(BaseUsers user)
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

    public Task<IEnumerable<BaseUsers>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BaseUsers> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<BaseUsers> GetUserByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(BaseUsers user)
    {
        throw new NotImplementedException();
    }
}

