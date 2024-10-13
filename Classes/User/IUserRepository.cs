using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My_SocNet_Win.Classes.User;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int id);
    Task<User> GetUserByUserNameAsync(string userName);
    Task<bool> ValidateUserCredentialsAsync(string userName, string password);
    Task CreateUserAsync(User user);
    Task<IEnumerable<User>> GetAllUsersAsync();

}
