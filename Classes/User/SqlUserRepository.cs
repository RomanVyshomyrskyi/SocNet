using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using My_SocNet_Win.Classes.DB.MSSQL;

namespace My_SocNet_Win.Classes.User
{
    public class SqlUserRepository : IUserRepository<BaseUsers>
    {
        private readonly MssqlService _mssqlService;
        private readonly SqlConnection _connection;

        public SqlUserRepository(MssqlService mssqlService)
        {
            _mssqlService = mssqlService;
            _connection = _mssqlService.GetConnection();
        }

        public async Task<BaseUsers> GetUserByIdAsync(int id)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var user = new BaseUsers
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            DateOfCreation = reader.GetDateTime(reader.GetOrdinal("DateOfCreation")),
                            LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin")),
                            ImgBinary = reader.GetString(reader.GetOrdinal("ImgBinary"))
                        };
                        _connection.Close();
                        return user;
                    }
                }
                _connection.Close();
            }
            return null;
        }

        public async Task<string> GetUserNameByIdAsync(int id)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT UserName FROM Users WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                var userName = await command.ExecuteScalarAsync() as string;
                _connection.Close();

                return userName;
            }
        }

        public async Task<IEnumerable<BaseUsers>> GetAllUsersAsync()
        {
            var users = new List<BaseUsers>();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users";

                _connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new BaseUsers
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            DateOfCreation = reader.GetDateTime(reader.GetOrdinal("DateOfCreation")),
                            LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin")),
                            ImgBinary = reader.GetString(reader.GetOrdinal("ImgBinary"))
                        };
                        users.Add(user);
                    }
                }
                _connection.Close();
            }
            return users;
        }

        public async Task AddUserAsync(BaseUsers user)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Users (UserName, Email, PasswordHash, DateOfCreation, LastLogin, ImgBinary)
                    VALUES (@UserName, @Email, @PasswordHash, @DateOfCreation, @LastLogin, @ImgBinary);
                    SELECT SCOPE_IDENTITY();";
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.Password); // Ensure this is hashed
                command.Parameters.AddWithValue("@DateOfCreation", user.DateOfCreation);
                command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
                command.Parameters.AddWithValue("@ImgBinary", user.ImgBinary);

                _connection.Open();
                user.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                _connection.Close();
            }
        }

        public async Task UpdateUserAsync(BaseUsers user)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE Users
                    SET UserName = @UserName,
                        Email = @Email,
                        PasswordHash = @PasswordHash,
                        DateOfCreation = @DateOfCreation,
                        LastLogin = @LastLogin,
                        ImgBinary = @ImgBinary
                    WHERE Id = @Id";
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.Password); // Ensure this is hashed
                command.Parameters.AddWithValue("@DateOfCreation", user.DateOfCreation);
                command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
                command.Parameters.AddWithValue("@ImgBinary", user.ImgBinary);
                command.Parameters.AddWithValue("@Id", user.Id);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
                _connection.Close();
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Users WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
                _connection.Close();
            }
        }

        public async Task EnsureAdminExistsAsync()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM Users WHERE UserName = 'admin')
                BEGIN
                    INSERT INTO Users (UserName, Email, PasswordHash, DateOfCreation, LastLogin, ImgBinary)
                    VALUES (@UserName, @Email, @PasswordHash, @DateOfCreation, @LastLogin, @ImgBinary);
                    SELECT SCOPE_IDENTITY();
                END";
                command.Parameters.AddWithValue("@UserName", "admin");
                command.Parameters.AddWithValue("@Email", "admin@admin.com");
                command.Parameters.AddWithValue("@PasswordHash", "admin"); // Replace with actual hashed password
                command.Parameters.AddWithValue("@DateOfCreation", DateTime.UtcNow);
                command.Parameters.AddWithValue("@LastLogin", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ImgBinary", string.Empty);

                _connection.Open();
                var adminId = Convert.ToInt32(await command.ExecuteScalarAsync()); // Get the generated Id for the admin user
                _connection.Close();

                if (adminId > 0)
                {
                    using (var roleCommand = _connection.CreateCommand())
                    {
                        roleCommand.CommandText = @"
                        IF NOT EXISTS (SELECT * FROM Roles WHERE Role = 'admin')
                        BEGIN
                            INSERT INTO Roles (Role)
                            VALUES ('admin');
                        END";
                        _connection.Open();
                        await roleCommand.ExecuteNonQueryAsync();
                        _connection.Close();
                    }
                }
            }
        }

        public async Task<BaseUsers> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordHash", password); // Ensure this is hashed

                _connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var user = new BaseUsers
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            DateOfCreation = reader.GetDateTime(reader.GetOrdinal("DateOfCreation")),
                            LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin")),
                            ImgBinary = reader.GetString(reader.GetOrdinal("ImgBinary"))
                        };
                        _connection.Close();
                        return user;
                    }
                }
                _connection.Close();
            }
            return null;
        }
    }
}