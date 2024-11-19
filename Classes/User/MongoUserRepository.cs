using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace My_SocNet_Win.Classes.User
{
    public class MongoUserRepository : IUserRepository<BaseUsers>
    {
        private readonly IMongoCollection<BaseUsers> _collection;
        private readonly IMongoCollection<Counter> _counterCollection;

        public MongoUserRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BaseUsers>("Users");
            _counterCollection = database.GetCollection<Counter>("Counters");
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
            user.Id = await GetNextSequenceValueAsync("UserId");
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
                    Id = await GetNextSequenceValueAsync("UserId"),
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

        private async Task<int> GetNextSequenceValueAsync(string sequenceName)
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, sequenceName);
            var update = Builders<Counter>.Update.Inc(c => c.SequenceValue, 1);
            var options = new FindOneAndUpdateOptions<Counter>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var counter = await _counterCollection.FindOneAndUpdateAsync(filter, update, options);
            return counter.SequenceValue;
        }
    }

    public class Counter
    {
        public string Id { get; set; }
        public int SequenceValue { get; set; }
    }
}