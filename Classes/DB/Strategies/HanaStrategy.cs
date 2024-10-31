using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using My_SocNet_Win.Classes.DB.HANA;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Classes.DB.Strategies
{
    public class HanaStrategy : IDatabaseStrategy
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var hanaConnectionString = configuration.GetConnectionString("HANA");
            if (string.IsNullOrEmpty(hanaConnectionString))
            {
                throw new ArgumentNullException(nameof(configuration), "HANA connection string cannot be null or empty.");
            }
            var hanaService = new HanaService(hanaConnectionString);
            services.AddSingleton(hanaService);
            services.AddScoped<IUserRepository<BaseUsers>, HanaUserRepository>();
            services.AddScoped<IDatabaseService, HanaService>();
        }

        public void EnsureAdminUserExists(IServiceProvider services)
        {
            var hanaUserRepository = services.GetService<IUserRepository<BaseUsers>>();
            if (hanaUserRepository != null)
            {
                hanaUserRepository.EnsureAdminExistsAsync().Wait();
            }
        }
    }
}
