using SemanticKernelLearning.Models;
using SemanticKernelLearning.Repositories;
using SemanticKernelLearning.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticKernelLearning.Tests.Services
{
    [TestFixture]
    public sealed class InventoryServiceTests
    {
        private InventoryService _service = null!;

        [SetUp]
        public void SetUp()
        {
            IInventoryRepository repository = new FakeInventoryRepository();
            _service = new InventoryService(repository);
        }

        [Test]
        public async Task GetStockQuantityAsync_ReturnsProductDetails_WhenProductExists()
        {
            var result = await _service.GetStockQuantityAsync(101);

            Assert.That(result, Does.Contain("Laptop"));
            Assert.That(result, Does.Contain("45"));
        }

        [Test]
        public async Task GetStockQuantityAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var result = await _service.GetStockQuantityAsync(999);

            Assert.That(result, Does.Contain("was not found"));
        }

        [Test]
        public async Task GetStockQuantityAsync_RejectsInvalidProductId()
        {
            var result = await _service.GetStockQuantityAsync(0);

            Assert.That(result, Is.EqualTo("Product ID must be greater than zero."));
        }
    }

    internal sealed class FakeInventoryRepository : IInventoryRepository
    {
        private readonly List<Product> _products =
        [
            new Product { Id = 101, Name = "Laptop", Quantity = 45 },
        new Product { Id = 102, Name = "Keyboard", Quantity = 120 }
        ];

        public Task<Product?> GetByIdAsync(
            int productId,
            CancellationToken cancellationToken = default)
        {
            Product? product = _products.FirstOrDefault(p => p.Id == productId);
            return Task.FromResult(product);
        }

        public Task<IReadOnlyList<Product>> SearchAsync(
            string searchTerm,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Product> products = _products
                .Where(p => p.Name.Contains(
                    searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult(products);
        }
    }
}
