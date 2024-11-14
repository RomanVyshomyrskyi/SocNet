using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using My_SocNet_Win.Classes.DB.MongoDB;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Classes.DB.Strategies
{
    public class MongoDbStrategy : IDatabaseStrategy
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("MongoDB");
            if (string.IsNullOrEmpty(mongoConnectionString))
            {
                throw new ArgumentNullException(nameof(mongoConnectionString), "MongoDB connection string cannot be null or empty.");
            }
            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase("MySite"); 
            services.AddSingleton(mongoDatabase);
            services.AddScoped<IUserRepository<BaseUsers>, MongoUserRepository>();
            services.AddScoped<IDatabaseService>(provider => new MongoDbService(mongoConnectionString));
        }

        public void EnsureAdminUserExists(IServiceProvider services)
        {
            var mongoUserRepository = services.GetService<IUserRepository<BaseUsers>>();
            if (mongoUserRepository != null)
            {
                mongoUserRepository.EnsureAdminExistsAsync().Wait();
            }
        }
    }
}
