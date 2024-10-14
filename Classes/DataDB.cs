using System;

namespace My_SocNet_Win.Classes;

public class DataDB
{
    protected static string GetDatabaseType(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetValue<string>("DatabaseType") ?? string.Empty;
    }

}
