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
            _mssqlService.EnsureDatabaseCreated();
        }

        public async Task AddUserAsync(BaseUsers user)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                INSERT INTO Users (UserName, Email, PasswordHash, DateOfCreation, LastLogin, ImgBinary)
                VALUES (@UserName, @Email, @PasswordHash, @DateOfCreation, @LastLogin, @ImgBinary);
                SELECT SCOPE_IDENTITY();"; // Get the newly generated Id
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.Password);
                command.Parameters.AddWithValue("@DateOfCreation", user.DateOfCreation);
                command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
                command.Parameters.AddWithValue("@ImgBinary", user.ImgBinary);

                _connection.Open();
                user.Id = Convert.ToInt32(await command.ExecuteScalarAsync()); // Set the generated Id to the user object
                _connection.Close();
            }

            foreach (var role in user.Roles)
            {
                int roleId = await GetRoleIdAsync(role);
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO UserRoles (UserId, RoleId)
                    VALUES (@UserId, @RoleId)";
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    command.Parameters.AddWithValue("@RoleId", roleId);

                    _connection.Open();
                    await command.ExecuteNonQueryAsync();
                    _connection.Close();
                }
            }

            foreach (var friendId in user.Friends)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO UserFriends (UserId, FriendId)
                    VALUES (@UserId, @FriendId)";
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    command.Parameters.AddWithValue("@FriendId", friendId);

                    _connection.Open();
                    await command.ExecuteNonQueryAsync();
                    _connection.Close();
                }
            }
        }

        private async Task<int> GetRoleIdAsync(string role)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM Roles WHERE Role = @Role)
                BEGIN
                    INSERT INTO Roles (Role)
                    VALUES (@Role);
                END;
                SELECT Id FROM Roles WHERE Role = @Role;";
                command.Parameters.AddWithValue("@Role", role);

                _connection.Open();
                int roleId = (int)await command.ExecuteScalarAsync();
                _connection.Close();

                return roleId;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                DELETE FROM Users WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
                _connection.Close();
            }
        }

        public async Task<BaseUsers> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            SqlUsers user = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                SELECT u.*, r.Role, f.FriendId
                FROM Users u
                LEFT JOIN UserRoles ur ON u.Id = ur.UserId
                LEFT JOIN Roles r ON ur.RoleId = r.Id
                LEFT JOIN UserFriends f ON u.Id = f.UserId
                WHERE u.Email = @Email AND u.PasswordHash = @PasswordHash";
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordHash", password);

                _connection.Open();
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
                            ImgBinary = reader["ImgBinary"].ToString(),
                            Roles = new List<string>(),
                            Friends = new List<int>()
                        };

                        do
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Role")))
                            {
                                user.Roles.Add(reader["Role"].ToString());
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("FriendId")))
                            {
                                user.Friends.Add((int)reader["FriendId"]);
                            }
                        } while (await reader.ReadAsync());
                    }
                }
                _connection.Close();
            }

            return user;
        }

        public async Task<IEnumerable<BaseUsers>> GetAllUsersAsync()
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

        public async Task<BaseUsers> GetUserByIdAsync(int id)
        {
            SqlUsers user = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                SELECT * FROM Users WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
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
                _connection.Close();
            }

            return user;
        }

        public async Task UpdateUserAsync(BaseUsers user)
        {
            using (var command = _connection.CreateCommand())
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

                _connection.Open();
                await command.ExecuteNonQueryAsync();
                _connection.Close();
            }

            // Update roles
            using (var deleteCommand = _connection.CreateCommand())
            {
                deleteCommand.CommandText = @"
                DELETE FROM UserRoles WHERE UserId = @UserId";
                deleteCommand.Parameters.AddWithValue("@UserId", user.Id);

                _connection.Open();
                await deleteCommand.ExecuteNonQueryAsync();
                _connection.Close();
            }

            foreach (var role in user.Roles)
            {
                int roleId = await GetRoleIdAsync(role);
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO UserRoles (UserId, RoleId)
                    VALUES (@UserId, @RoleId)";
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    command.Parameters.AddWithValue("@RoleId", roleId);

                    _connection.Open();
                    await command.ExecuteNonQueryAsync();
                    _connection.Close();
                }
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

                    using (var userRoleCommand = _connection.CreateCommand())
                    {
                        userRoleCommand.CommandText = @"
                        IF NOT EXISTS (SELECT * FROM UserRoles WHERE UserId = @UserId AND RoleId = (SELECT Id FROM Roles WHERE Role = 'admin'))
                        BEGIN
                            INSERT INTO UserRoles (UserId, RoleId)
                            VALUES (@UserId, (SELECT Id FROM Roles WHERE Role = 'admin'))
                        END";
                        userRoleCommand.Parameters.AddWithValue("@UserId", adminId);

                        _connection.Open();
                        await userRoleCommand.ExecuteNonQueryAsync();
                        _connection.Close();
                    }
                }
            }
        }
    }
}