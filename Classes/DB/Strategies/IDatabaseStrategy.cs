using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace My_SocNet_Win.Classes.DB.Strategies
{
    public interface IDatabaseStrategy
    {
        void Configure(IServiceCollection services, IConfiguration configuration);
        void EnsureAdminUserExists(IServiceProvider services);
    }
}
