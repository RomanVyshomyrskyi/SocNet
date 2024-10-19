using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My_SocNet_Win.Classes.User;

public interface IUserRepository<T> where T : BaseUsers
{
    Task<T> GetUserByIdAsync(string id);
    Task<IEnumerable<T>> GetAllUsersAsync();
    Task AddUserAsync(T user);
    Task UpdateUserAsync(T user);
    Task DeleteUserAsync(string id);
    Task EnsureAdminExistsAsync();
}
