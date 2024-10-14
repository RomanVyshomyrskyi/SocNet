using My_SocNet_Win.Classes.DB;
using My_SocNet_Win.Classes.DB.HANA;
using My_SocNet_Win.Classes.DB.MongoDB;
using My_SocNet_Win.Classes.DB.MSSQL;
using My_SocNet_Win.Classes.DB.Neo4J;
using My_SocNet_Win.Classes.DB.Redis;
using My_SocNet_Win.Classes.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Add configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Get the database type from the configuration (appsettings.json)
var databaseType = builder.Configuration.GetValue<string>("DatabaseType");

switch (databaseType)
{
    case "MongoDb":
        var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
        if (string.IsNullOrEmpty(mongoConnectionString))
        {
            throw new ArgumentNullException(nameof(mongoConnectionString), "MongoDb connection string cannot be null or empty.");
        }
        var mongoDbService = new MongoDbService(mongoConnectionString);
        builder.Services.AddSingleton(mongoDbService);
        builder.Services.AddSingleton<IUserRepository>(new MongoUserRepository(mongoDbService));
        break;
    case "Redis":
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new ArgumentNullException(nameof(redisConnectionString), "Redis connection string cannot be null or empty.");
        }
        var redisService = new RedisService(redisConnectionString);
        builder.Services.AddSingleton(redisService);
        builder.Services.AddSingleton<IUserRepository>(new RedisUserRepository(redisService));
        break;
    case "HANA":
        var hanaConnectionString = builder.Configuration.GetConnectionString("HANA");
        if (string.IsNullOrEmpty(hanaConnectionString))
        {
            throw new ArgumentNullException(nameof(hanaConnectionString), "HANA connection string cannot be null or empty.");
        }
        var hanaService = new HanaService(hanaConnectionString);
        builder.Services.AddSingleton(hanaService);
        builder.Services.AddSingleton<IUserRepository>(new HanaUserRepository(hanaService));
        break;
    case "Neo4J":
        var neo4jConnectionString = builder.Configuration.GetConnectionString("Neo4J");
        if (string.IsNullOrEmpty(neo4jConnectionString))
        {
            throw new ArgumentNullException(nameof(neo4jConnectionString), "Neo4J connection string cannot be null or empty.");
        }
        var neo4jService = new Neo4jService(neo4jConnectionString);
        builder.Services.AddSingleton(neo4jService);
        builder.Services.AddSingleton<IUserRepository>(new Neo4jUserRepository(neo4jService));
        break;
    case "MSSQL":
        var mssqlConnectionString = builder.Configuration.GetConnectionString("MSSQL");
        if (string.IsNullOrEmpty(mssqlConnectionString))
        {
            throw new ArgumentNullException(nameof(mssqlConnectionString), "MSSQL connection string cannot be null or empty.");
        }
        var mssqlService = new MssqlService(mssqlConnectionString);
        builder.Services.AddSingleton(mssqlService);
        builder.Services.AddSingleton<IUserRepository>(new MssqlUserRepository(mssqlService));
        break;
    default:
        throw new Exception("Unsupported database type");
}


//Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
