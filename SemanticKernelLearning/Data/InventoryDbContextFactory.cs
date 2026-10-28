
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SemanticKernelLearning.Data;

namespace SemanticKernelLearning.Data;

public sealed class InventoryDbContextFactory
    : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .Build();

        var connectionString =
            configuration.GetConnectionString("InventoryDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'InventoryDatabase' was not found.");

        var optionsBuilder =
            new DbContextOptionsBuilder<InventoryDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new InventoryDbContext(optionsBuilder.Options);
    }
}
