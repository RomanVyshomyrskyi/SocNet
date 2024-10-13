using My_SocNet_Win.Classes.DB;
using My_SocNet_Win.Classes.DB.HANA;
using My_SocNet_Win.Classes.DB.MongoDB;
using My_SocNet_Win.Classes.DB.Neo4J;
using My_SocNet_Win.Classes.DB.Redis;

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
        builder.Services.AddSingleton<IDatabaseService>(new MongoDbService(mongoConnectionString));
        break;
    case "Redis":
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
        builder.Services.AddSingleton<IDatabaseService>(new RedisService(redisConnectionString));
        break;
    case "HANA":
        var hanaConnectionString = builder.Configuration.GetConnectionString("HANA");
        builder.Services.AddSingleton<IDatabaseService>(new HanaService(hanaConnectionString));
        break;
    case "Neo4J":
        var neo4jConnectionString = builder.Configuration.GetConnectionString("Neo4J");
        builder.Services.AddSingleton<IDatabaseService>(new Neo4jService(neo4jConnectionString));
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
