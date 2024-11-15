using MongoDB.Driver;

namespace My_SocNet_Win.Classes.User
{
    public class MongoUserRepository : IUserRepository<BaseUsers>
    {
        private readonly IMongoCollection<BaseUsers> _collection;

        public MongoUserRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BaseUsers>("Users");
        }

        public async Task<BaseUsers> GetUserByIdAsync(int id)
        {
            return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BaseUsers>> GetAllUsersAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddUserAsync(BaseUsers user)
        {
            await _collection.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(BaseUsers user)
        {
            await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _collection.DeleteOneAsync(u => u.Id == id);
        }

        public async Task EnsureAdminExistsAsync()
        {
            var adminUser = await _collection.Find(u => u.UserName == "admin").FirstOrDefaultAsync();
            if (adminUser == null)
            {
                var admin = new MongoUsers
                {
                    Id = 1,
                    UserName = "admin",
                    Email = "admin@admin.com",
                    Password = "admin", // Replace with actual hashed password
                    DateOfCreation = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    ImgBinary = string.Empty,
                    Roles = new List<string> { "admin" }
                };
                await AddUserAsync(admin);
            }
        }

        public async Task<BaseUsers> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            return await _collection.Find(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
        }
    }
}
