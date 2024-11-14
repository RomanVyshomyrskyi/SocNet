using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using My_SocNet_Win.Classes.DB.MSSQL;
using My_SocNet_Win.Classes.User;
using My_SocNet_Win.Classes.Posts;
using My_SocNet_Win.Classes.Comment;

namespace My_SocNet_Win.Classes.DB.Strategies
{
    public class MssqlStrategy : IDatabaseStrategy
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var mssqlConnectionString = configuration.GetConnectionString("MSSQL");
            if (string.IsNullOrEmpty(mssqlConnectionString))
            {
                throw new ArgumentNullException(nameof(mssqlConnectionString), "MSSQL connection string cannot be null or empty.");
            }
            var mssqlService = new MssqlService(mssqlConnectionString);
            services.AddSingleton(mssqlService);
            services.AddScoped<IDatabaseService>(provider => provider.GetRequiredService<MssqlService>());
            services.AddScoped<IUserRepository<BaseUsers>, SqlUserRepository>(provider => 
                new SqlUserRepository(provider.GetRequiredService<MssqlService>()));
            services.AddScoped<IPostRepository<BasePost>, SQLPostReposetory>(provider =>
                new SQLPostReposetory(provider.GetRequiredService<MssqlService>()));
            services.AddScoped<ICommentRepository<BaseComment>, SQLCommentRepository>(provider =>
                new SQLCommentRepository(provider.GetRequiredService<MssqlService>()));
        }

        public void EnsureAdminUserExists(IServiceProvider services)
        {
            var sqlUserRepository = services.GetService<IUserRepository<BaseUsers>>();
            if (sqlUserRepository != null)
            {
                sqlUserRepository.EnsureAdminExistsAsync().Wait();
            }
        }
    }

}
