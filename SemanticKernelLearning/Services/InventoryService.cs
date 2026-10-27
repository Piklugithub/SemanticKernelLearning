
using SemanticKernelLearning.Models;
using SemanticKernelLearning.Repositories;

namespace SemanticKernelLearning.Services;

public sealed class InventoryService
{
    private readonly IInventoryRepository _repository;

    public InventoryService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> GetStockQuantityAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        if (productId <= 0)
            return "Product ID must be greater than zero.";

        Product? product = await _repository.GetByIdAsync(
            productId, cancellationToken);

        return product is null
            ? $"Product ID {productId} was not found."
            : $"Product ID: {product.Id}, Name: {product.Name}, Available quantity: {product.Quantity}.";
    }

    public async Task<string> SearchProductsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return "Please provide a product name to search for.";

        var products = await _repository.SearchAsync(
            searchTerm.Trim(), cancellationToken);

        return products.Count == 0
            ? $"No products matched '{searchTerm}'."
            : string.Join(
                Environment.NewLine,
                products.Select(p =>
                    $"ID: {p.Id}, Name: {p.Name}, Stock: {p.Quantity}"));
    }
}
