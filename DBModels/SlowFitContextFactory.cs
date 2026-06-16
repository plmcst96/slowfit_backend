using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace slowfit.DBModels;

public sealed class SlowFitContextFactory : IDesignTimeDbContextFactory<SlowFitContext>
{
    public SlowFitContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("dbConnectionString") ??
            configuration["ConnectionStrings:dbConnectionString"] ??
            configuration["ConnectionStrings__dbConnectionString"] ??
            configuration["ConnectionStrings__SlowFitDb"];

        if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
        {
            connectionString = "Server=localhost,1433;Database=SlowFit;User Id=sa;Password=Slowfit123!;TrustServerCertificate=True;";
        }

        var optionsBuilder = new DbContextOptionsBuilder<SlowFitContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SlowFitContext(optionsBuilder.Options);
    }
}
