
using SemanticKernelLearning.Models;

namespace SemanticKernelLearning.Repositories;

public sealed class InMemoryInventoryRepository
    : IInventoryRepository
{
    private readonly List<Product> _products =
    [
        new(101, "Laptop", 45),
        new(102, "Keyboard", 120),
        new(103, "Mouse", 80),
        new(104, "Monitor", 25)
    ];

    public Task<Product?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Product? product = _products
            .FirstOrDefault(p => p.Id == productId);

        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<Product>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Product> matches = _products
            .Where(p => p.Name.Contains(
                searchTerm,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(matches);
    }
}
