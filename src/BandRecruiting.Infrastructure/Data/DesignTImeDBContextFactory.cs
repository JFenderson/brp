// src/BandRecruiting.Infrastructure/Data/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BandRecruiting.Infrastructure.Data;

public class DesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // go up to the solution root so it finds appsettings.Development.json
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var cfg = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("src/BandRecruiting.API/appsettings.json", optional: true)
            .AddJsonFile("src/BandRecruiting.API/appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(cfg.GetConnectionString("DefaultConnection"))
            .Options;

        return new ApplicationDbContext(options);
    }
}
