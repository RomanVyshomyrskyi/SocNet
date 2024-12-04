using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My_SocNet_Win.Classes.User;

public interface IUserRepository<T> where T : BaseUsers
{
    Task<T> GetUserByIdAsync(int id);
    Task<string> GetUserNameByIdAsync(int id);
    Task<IEnumerable<T>> GetAllUsersAsync();
    Task AddUserAsync(T user);
    Task UpdateUserAsync(T user);
    Task DeleteUserAsync(int id);
    Task EnsureAdminExistsAsync();
    Task<T> GetUserByEmailAndPasswordAsync(string email, string password);
    
}
