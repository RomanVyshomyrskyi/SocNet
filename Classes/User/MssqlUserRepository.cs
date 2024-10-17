using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using My_SocNet_Win.Classes.DB.MSSQL;

namespace My_SocNet_Win.Classes.User
{
    public class MssqlUserRepository : IUserRepository
    {
        private readonly MssqlService _mssqlService;

        public MssqlUserRepository(MssqlService mssqlService)
        {
            _mssqlService = mssqlService;
        }

        public async Task<Users> GetUserByUserNameAsync(string userName)
        {
            using var connection = new SqlConnection(_mssqlService.ConnectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT * FROM Users WHERE UserName = @UserName", connection);
            command.Parameters.AddWithValue("@UserName", userName);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Users
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    Friends = reader.IsDBNull(reader.GetOrdinal("Friends")) ? null : new List<int>(Array.ConvertAll(reader.GetString(reader.GetOrdinal("Friends")).Split(','), int.Parse)),
                    DateOfCreation = reader.GetDateTime(reader.GetOrdinal("DateOfCreation")),
                    LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin")),
                    Roles = reader.IsDBNull(reader.GetOrdinal("Roles")) ? null : new List<string>(reader.GetString(reader.GetOrdinal("Roles")).Split(','))
                };
            }

            return null;
        }

        public async Task CreateUserAsync(Users user)
        {
            var query = "INSERT INTO Users (UserName, Password, Friends, DateOfCreation, LastLogin, Roles) VALUES (@UserName, @Password, @Friends, @DateOfCreation, @LastLogin, @Roles)";
            
            using (var connection = new SqlConnection(_mssqlService.ConnectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Friends", user.Friends != null ? string.Join(",", user.Friends) : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfCreation", user.DateOfCreation);
                    command.Parameters.AddWithValue("@LastLogin", user.LastLogin);
                    command.Parameters.AddWithValue("@Roles", user.Roles != null ? string.Join(",", user.Roles) : (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            var query = "SELECT * FROM Users";
            var users = new List<Users>();

            using (var connection = new SqlConnection(_mssqlService.ConnectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new Users
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Friends = reader.IsDBNull(reader.GetOrdinal("Friends")) ? null : new List<int>(Array.ConvertAll(reader.GetString(reader.GetOrdinal("Friends")).Split(','), int.Parse)),
                                DateOfCreation = reader.GetDateTime(reader.GetOrdinal("DateOfCreation")),
                                LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin")),
                                Roles = reader.IsDBNull(reader.GetOrdinal("Roles")) ? null : new List<string>(reader.GetString(reader.GetOrdinal("Roles")).Split(','))
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public Task<Users> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateUserCredentialsAsync(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}