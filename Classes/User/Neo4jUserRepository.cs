using System;

namespace My_SocNet_Win.Classes.User;

public class Neo4jUserRepository : IUserRepository<Neo4jUsers>
{
    //TODO: Implement Neo4j-specific methods
    public Task AddUserAsync(Neo4jUsers user)
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

    public Task<IEnumerable<Neo4jUsers>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Neo4jUsers> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<Neo4jUsers> GetUserByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(Neo4jUsers user)
    {
        throw new NotImplementedException();
    }
}

