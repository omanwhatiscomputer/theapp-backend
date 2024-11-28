using System;
using Microsoft.EntityFrameworkCore;

namespace UserService.Data;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetService<UserDbContext>()
            ?? throw new InvalidOperationException("Failed to retrieve UserDbContext from the service provider.");

        context.Database.Migrate();

    }
}
