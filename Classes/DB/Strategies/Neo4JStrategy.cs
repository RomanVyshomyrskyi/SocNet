using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using My_SocNet_Win.Classes.DB.Neo4J;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Classes.DB.Strategies
{
    public class Neo4JStrategy : IDatabaseStrategy
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var neo4jConnectionString = configuration.GetConnectionString("Neo4J");
            if (string.IsNullOrEmpty(neo4jConnectionString))
            {
                throw new ArgumentNullException(nameof(configuration), "Neo4J connection string cannot be null or empty.");
            }
            var neo4jService = new Neo4jService(neo4jConnectionString);
            services.AddSingleton(neo4jService);
            services.AddScoped<IUserRepository<BaseUsers>, Neo4jUserRepository>();
            services.AddScoped<IDatabaseService, Neo4jService>();
        }

        public void EnsureAdminUserExists(IServiceProvider services)
        {
            var neo4jUserRepository = services.GetService<IUserRepository<BaseUsers>>();
            if (neo4jUserRepository != null)
            {
                neo4jUserRepository.EnsureAdminExistsAsync().Wait();
            }
        }
    }
}
