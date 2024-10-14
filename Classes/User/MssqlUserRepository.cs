using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using My_SocNet_Win.Classes.DB.MSSQL;


namespace My_SocNet_Win.Classes.User;

public class MssqlUserRepository : IUserRepository
{ 
    private readonly SqlConnection _connection;

    public MssqlUserRepository(MssqlService mssqlService)
    {
        _connection = mssqlService.GetConnection();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var query = "SELECT * FROM Users WHERE Id = @Id";
        var command = new SqlCommand(query, _connection);
        command.Parameters.AddWithValue("@Id", id);

        _connection.Open();
        var reader = await command.ExecuteReaderAsync();
        User user = null;
        if (reader.Read())
        {
            user = new User
            {
                Id = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Password = reader.GetString(2),
                Friends = reader.IsDBNull(3) ? null : new List<int>(Array.ConvertAll(reader.GetString(3).Split(','), int.Parse)),
                DateOfCreation = reader.GetDateTime(4),
                LastLogin = reader.GetDateTime(5),
                Roles = reader.IsDBNull(6) ? null : new List<string>(reader.GetString(6).Split(','))
            };
        }
        _connection.Close();
        return user;
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
        var query = "SELECT * FROM Users WHERE UserName = @UserName";
        var command = new SqlCommand(query, _connection);
        command.Parameters.AddWithValue("@UserName", userName);

        _connection.Open();
        var reader = await command.ExecuteReaderAsync();
        User user = null;
        if (reader.Read())
        {
            user = new User
            {
                Id = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Password = reader.GetString(2),
                Friends = reader.IsDBNull(3) ? null : new List<int>(Array.ConvertAll(reader.GetString(3).Split(','), int.Parse)),
                DateOfCreation = reader.GetDateTime(4),
                LastLogin = reader.GetDateTime(5),
                Roles = reader.IsDBNull(6) ? null : new List<string>(reader.GetString(6).Split(','))
            };
        }
        _connection.Close();
        return user;
    }

    public async Task<bool> ValidateUserCredentialsAsync(string userName, string password)
    {
        var user = await GetUserByUserNameAsync(userName);
        return user != null && user.Password == password;
    }

    public async Task CreateUserAsync(User user)
    {
        var query = "INSERT INTO Users (UserName, Password, Friends, DateOfCreation, LastLogin, Roles) VALUES (@UserName, @Password, @Friends, @DateOfCreation, @LastLogin, @Roles)";
        var command = new SqlCommand(query, _connection);
        command.Parameters.AddWithValue("@UserName", user.UserName);
        command.Parameters.AddWithValue("@Password", user.Password);
        command.Parameters.AddWithValue("@Friends", user.Friends != null ? string.Join(",", user.Friends) : (object)DBNull.Value);
        command.Parameters.AddWithValue("@DateOfCreation", user.DateOfCreation);
        command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
        command.Parameters.AddWithValue("@Roles", user.Roles != null ? string.Join(",", user.Roles) : (object)DBNull.Value);

        _connection.Open();
        await command.ExecuteNonQueryAsync();
        _connection.Close();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var query = "SELECT * FROM Users";
        var command = new SqlCommand(query, _connection);

        _connection.Open();
        var reader = await command.ExecuteReaderAsync();
        var users = new List<User>();
        while (reader.Read())
        {
            var user = new User
            {
                Id = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Password = reader.GetString(2),
                Friends = reader.IsDBNull(3) ? null : new List<int>(Array.ConvertAll(reader.GetString(3).Split(','), int.Parse)),
                DateOfCreation = reader.GetDateTime(4),
                LastLogin = reader.GetDateTime(5),
                Roles = reader.IsDBNull(6) ? null : new List<string>(reader.GetString(6).Split(','))
            };
            users.Add(user);
        }
        _connection.Close();
        return users;
    }

}
