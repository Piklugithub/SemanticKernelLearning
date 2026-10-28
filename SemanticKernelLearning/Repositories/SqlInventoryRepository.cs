
using Microsoft.EntityFrameworkCore;
using SemanticKernelLearning.Data;
using SemanticKernelLearning.Models;

namespace SemanticKernelLearning.Repositories;

public sealed class SqlInventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _dbContext;

    public SqlInventoryRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.Id == productId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Array.Empty<Product>();

        searchTerm = searchTerm.Trim();

        return await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Name.Contains(searchTerm))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
