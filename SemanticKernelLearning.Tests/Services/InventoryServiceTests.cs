using SemanticKernelLearning.Services;

namespace SemanticKernelLearning.Tests.Services;

public sealed class InventoryServiceTests
{
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        var repository = new FakeInventoryRepository();
        _service = new InventoryService(repository);
    }

    [Fact]
    public async Task GetStockQuantityAsync_ExistingProduct_ReturnsQuantity()
    {
        // Arrange
        const int productId = 101;

        // Act
        var result = await _service.GetStockQuantityAsync(productId);

        // Assert
        Assert.Equal(
            "Product ID: 101, Name: Laptop, Available quantity: 45.",
            result);
    }

    [Fact]
    public async Task GetStockQuantityAsync_UnknownProduct_ReturnsNotFound()
    {
        // Act
        var result = await _service.GetStockQuantityAsync(999);

        // Assert
        Assert.Equal("Product ID 999 was not found.", result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetStockQuantityAsync_InvalidId_ReturnsValidationMessage(
        int productId)
    {
        // Act
        var result = await _service.GetStockQuantityAsync(productId);

        // Assert
        Assert.Equal("Product ID must be greater than zero.", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchProductsAsync_EmptyTerm_ReturnsValidationMessage(
        string searchTerm)
    {
        // Act
        var result = await _service.SearchProductsAsync(searchTerm);

        // Assert
        Assert.Equal(
            "Please provide a product name to search for.",
            result);
    }
}
