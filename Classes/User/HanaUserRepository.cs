using System;
using My_SocNet_Win.Classes.DB.HANA;
using Sap.Data.Hana;

namespace My_SocNet_Win.Classes.User;

public class HanaUserRepository : IUserRepository
{
private readonly HanaConnection _connection;

    public HanaUserRepository(HanaService hanaService)
    {
        _connection = hanaService.GetConnection();
    }

    public async Task<Users> GetUserByIdAsync(int id)
    {
        // Implementation for getting user by ID from SAP HANA
        throw new NotImplementedException();
    }

    public async Task<Users> GetUserByUserNameAsync(string userName)
    {
        // Implementation for getting user by UserName from SAP HANA
        throw new NotImplementedException();
    }

    public async Task<bool> ValidateUserCredentialsAsync(string userName, string password)
    {
        var user = await GetUserByUserNameAsync(userName);
        return user != null && user.Password == password;
    }

    public async Task CreateUserAsync(Users user)
    {
        user.DateOfCreation = DateTime.UtcNow;
        // Implementation for creating user in SAP HANA
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Users>> GetAllUsersAsync()
    {
        // Implementation for getting all users from SAP HANA
        throw new NotImplementedException();
    }

}
