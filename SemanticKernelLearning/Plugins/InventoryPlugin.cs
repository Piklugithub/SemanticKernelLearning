
using System.ComponentModel;
using Microsoft.SemanticKernel;
using SemanticKernelLearning.Services;

namespace SemanticKernelLearning.Plugins;

public sealed class InventoryPlugin
{
    private readonly InventoryService _inventoryService;

    public InventoryPlugin(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [KernelFunction("GetStockQuantity")]
    [Description("Gets the available stock quantity for a product by numeric product ID.")]
    public Task<string> GetStockQuantityAsync(
        [Description("The numeric product ID.")] int productId)
    {
        return _inventoryService.GetStockQuantityAsync(productId);
    }

    [KernelFunction("SearchProducts")]
    [Description("Searches inventory by product name and returns matching stock quantities.")]
    public Task<string> SearchProductsAsync(
        [Description("A product name or partial name.")] string searchTerm)
    {
        return _inventoryService.SearchProductsAsync(searchTerm);
    }

    [KernelFunction("GetStockByProductName")]
    [Description("Gets the current stock quantity for a product using its name. Use this when the user asks about a product by name, such as Laptop or Mouse.")]
    public Task<string> GetStockByProductNameAsync(
    [Description("The product name provided by the user, such as Laptop. Do not use a numeric product ID here.")]
    string productName)
    {
        return _inventoryService.GetStockByProductNameAsync(productName);
    }
}
