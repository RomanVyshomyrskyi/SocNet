using System;

namespace My_SocNet_Win.Classes.User;

public class HanaUserRepository : IUserRepository<HanaUsers>
{
    //TODO: Implement HANA-specific methods
    public Task AddUserAsync(HanaUsers user)
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

    public Task<IEnumerable<HanaUsers>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<HanaUsers> GetUserByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(HanaUsers user)
    {
        throw new NotImplementedException();
    }
}
