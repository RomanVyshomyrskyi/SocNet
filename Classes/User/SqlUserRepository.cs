using System;
using System.Data.SqlClient;
using My_SocNet_Win.Classes.DB.MSSQL;

namespace My_SocNet_Win.Classes.User;

public class SqlUserRepository : IUserRepository<SqlUsers>
{
    private readonly MssqlService _mssqlService;
    public SqlUserRepository(MssqlService mssqlService)
    {
        _mssqlService = mssqlService;
        _mssqlService.EnsureDatabaseCreated();
    }
    

    public async Task AddUserAsync(SqlUsers user)
    {
        using (var connection = _mssqlService.GetConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            INSERT INTO Users (Id, UserName, Email, PasswordHash, DateOfCreation, LastLogin, ImgBinary)
            VALUES (@Id, @UserName, @Email, @PasswordHash, @DateOfCreation, @LastLogin, @ImgBinary)";
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.Password);
            command.Parameters.AddWithValue("@DateOfCreation", user.DateOfCreation);
            command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
            command.Parameters.AddWithValue("@ImgBinary", user.ImgBinary);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        foreach (var role in user.Roles)
        {
            using (var connection = _mssqlService.GetConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                INSERT INTO UserRoles (UserId, Role)
                VALUES (@UserId, @Role)";
                command.Parameters.AddWithValue("@UserId", user.Id);
                command.Parameters.AddWithValue("@Role", role);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }
        }

        foreach (var friendId in user.Friends)
        {
            using (var connection = _mssqlService.GetConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                INSERT INTO UserFriends (UserId, FriendId)
                VALUES (@UserId, @FriendId)";
                command.Parameters.AddWithValue("@UserId", user.Id);
                command.Parameters.AddWithValue("@FriendId", friendId);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }
        }
    }

    public async Task DeleteUserAsync(string id)
    {
       using (var connection = _mssqlService.GetConnection())
       using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            DELETE FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
    }
    public async Task<SqlUsers> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        SqlUsers user = null;

        using (var connection = _mssqlService.GetConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            SELECT * FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", password);

            connection.Open();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    user = new SqlUsers
                    {
                        Id = (int)reader["Id"],
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["PasswordHash"].ToString(),
                        DateOfCreation = (DateTime)reader["DateOfCreation"],
                        LastLogin = (DateTime)reader["LastLogin"],
                        ImgBinary = reader["ImgBinary"].ToString()
                    };
                }
            }

            if (user != null)
            {
                user.Roles = new List<string>();
                user.Friends = new List<int>();

                using (var roleCommand = connection.CreateCommand())
                {
                    roleCommand.CommandText = @"
                    SELECT Role FROM UserRoles WHERE UserId = @UserId";
                    roleCommand.Parameters.AddWithValue("@UserId", user.Id);
                    using (var roleReader = await roleCommand.ExecuteReaderAsync())
                    {
                        while (await roleReader.ReadAsync())
                        {
                        user.Roles.Add(roleReader["Role"].ToString());
                        }
                    }
                }

                using (var friendCommand = connection.CreateCommand())
                {
                    friendCommand.CommandText = @"
                    SELECT FriendId FROM UserFriends WHERE UserId = @UserId";
                    friendCommand.Parameters.AddWithValue("@UserId", user.Id);

                    using (var friendReader = await friendCommand.ExecuteReaderAsync())
                    {
                        while (await friendReader.ReadAsync())
                        {
                        user.Friends.Add((int)friendReader["FriendId"]);
                        }
                    }
                }
            }

            connection.Close();
        }
        

        return user;
    }

    public async Task<IEnumerable<SqlUsers>> GetAllUsersAsync()
    {
        var users = new List<SqlUsers>();

        using (var connection = _mssqlService.GetConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM Users";

            connection.Open();
            using (var reader = await command.ExecuteReaderAsync())
            {
            while (await reader.ReadAsync())
            {
                users.Add(new SqlUsers
                {
                Id = (int)reader["Id"],
                UserName = reader["UserName"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["PasswordHash"].ToString(),
                DateOfCreation = (DateTime)reader["DateOfCreation"],
                LastLogin = (DateTime)reader["LastLogin"],
                ImgBinary = reader["ImgBinary"].ToString()
                });
            }
            }
            connection.Close();
        }

        return users;
    }

    public async Task<SqlUsers> GetUserByIdAsync(string id)
    {
        SqlUsers user = null;

        using (var connection = _mssqlService.GetConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            SELECT * FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using (var reader = await command.ExecuteReaderAsync())
            {
            if (await reader.ReadAsync())
            {
                user = new SqlUsers
                {
                Id = (int)reader["Id"],
                UserName = reader["UserName"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["PasswordHash"].ToString(),
                DateOfCreation = (DateTime)reader["DateOfCreation"],
                LastLogin = (DateTime)reader["LastLogin"],
                ImgBinary = reader["ImgBinary"].ToString()
                };
            }
            }
            connection.Close();
        }

        return user;
    }

    public async Task UpdateUserAsync(SqlUsers user)
    {
        using (var connection = _mssqlService.GetConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            UPDATE Users
            SET UserName = @UserName, Email = @Email, PasswordHash = @PasswordHash, DateOfCreation = @DateOfCreation, LastLogin = @LastLogin, ImgBinary = @ImgBinary
            WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.Password);
            command.Parameters.AddWithValue("@DateOfCreation", user.DateOfCreation);
            command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
            command.Parameters.AddWithValue("@ImgBinary", user.ImgBinary);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
    }

 
    public async Task EnsureAdminExistsAsync()
    {
        using (var connection = _mssqlService.GetConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            IF NOT EXISTS (SELECT * FROM Users WHERE UserName = 'admin')
            BEGIN
                INSERT INTO Users (Id, UserName, Email, PasswordHash, DateOfCreation, LastLogin, ImgBinary)
                VALUES (@Id, @UserName, @Email, @PasswordHash, @DateOfCreation, @LastLogin, @ImgBinary)
            END";
            command.Parameters.AddWithValue("@Id", 1);
            command.Parameters.AddWithValue("@UserName", "admin");
            command.Parameters.AddWithValue("@Email", "admin@admin.com");
            command.Parameters.AddWithValue("@PasswordHash", "admin"); // Replace with actual hashed password
            command.Parameters.AddWithValue("@DateOfCreation", DateTime.UtcNow);
            command.Parameters.AddWithValue("@LastLogin", DateTime.UtcNow);
            command.Parameters.AddWithValue("@ImgBinary", string.Empty);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        using (var connection = _mssqlService.GetConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            IF NOT EXISTS (SELECT * FROM UserRoles WHERE UserId = @UserId AND Role = @Role)
            BEGIN
                INSERT INTO UserRoles (UserId, Role)
                VALUES (@UserId, @Role)
            END";
            command.Parameters.AddWithValue("@UserId", 1);
            command.Parameters.AddWithValue("@Role", "admin");

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
    }
}
