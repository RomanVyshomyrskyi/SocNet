using My_SocNet_Win;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Add configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Get the database type from the configuration (appsettings.json)
var databaseType = builder.Configuration.GetValue<string>("DatabaseType");

if (string.IsNullOrEmpty(databaseType))
{
    throw new ArgumentNullException(nameof(databaseType), "DatabaseType configuration is missing or empty.");
}

DatabaseConfigurator.ConfigureDatabaseServices(builder.Services, builder.Configuration, databaseType);


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
