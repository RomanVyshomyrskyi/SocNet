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
            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase("YourDatabaseName");
            services.AddSingleton(mongoDatabase);
            services.AddScoped<IUserRepository<BaseUsers>, MongoUserRepository>();
            services.AddScoped<IDatabaseService, MongoDbService>();
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
