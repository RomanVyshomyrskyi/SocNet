using System;

namespace My_SocNet_Win.Classes.User;

public class HanaUserRepository : IUserRepository<BaseUsers>
{
    //TODO: Implement HANA-specific methods
    public Task AddUserAsync(BaseUsers user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(int id)
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

    public Task<BaseUsers> GetUserByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserNameByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(BaseUsers user)
    {
        throw new NotImplementedException();
    }
}
