
using SemanticKernelLearning.Models;

namespace SemanticKernelLearning.Repositories;

public sealed class InMemoryInventoryRepository
    : IInventoryRepository
{
    private readonly List<Product> _products =
    [
        new Product
        {
            Id = 1,
            Name = "Laptop",
            Quantity = 45
        },
        new Product
        {
            Id = 2,
            Name = "Keyboard",
            Quantity = 120
        },
        new Product
        {
            Id = 3,
            Name = "Mouse",
            Quantity = 80
        }
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
