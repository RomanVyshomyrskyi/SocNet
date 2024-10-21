using My_SocNet_Win;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.DB;
using My_SocNet_Win.Classes.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Add configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Add the SiteSettings configuration
builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection("SiteSettings"));

// Get the database type from the configuration (appsettings.json)
var databaseType = builder.Configuration.GetValue<string>("DatabaseType");

if (string.IsNullOrEmpty(databaseType))
{
    throw new ArgumentNullException(nameof(databaseType), "DatabaseType configuration is missing or empty.");
}

DatabaseConfigurator.ConfigureDatabase(builder.Services, builder.Configuration, databaseType);


//Build the app
var app = builder.Build();

// Ensure the database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var databaseService = services.GetRequiredService<IDatabaseService>();
        databaseService.EnsureDatabaseCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Ensure the database and admin user exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
     DatabaseConfigurator.EnsureAdminUserExists(services, databaseType);
}

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
