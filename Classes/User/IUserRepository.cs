using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My_SocNet_Win.Classes.User;

public interface IUserRepository
{
    Task<Users> GetUserByIdAsync(int id);
    Task<Users> GetUserByUserNameAsync(string userName);
    Task<bool> ValidateUserCredentialsAsync(string userName, string password);
    Task CreateUserAsync(Users user);
    Task<IEnumerable<Users>> GetAllUsersAsync();

}
