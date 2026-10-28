using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using SemanticKernelLearning.Data;
using SemanticKernelLearning.Plugins;
using SemanticKernelLearning.Repositories;
using SemanticKernelLearning.Services;

#pragma warning disable SKEXP0070

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var builder = Kernel.CreateBuilder();

var ollamaEndpoint =
    configuration["Ollama:Endpoint"] ?? "http://localhost:11434";

var ollamaModelId =
    configuration["Ollama:ModelId"] ?? "qwen2.5:3b";

builder.AddOllamaChatCompletion(
    modelId: ollamaModelId,
    endpoint: new Uri(ollamaEndpoint));

var connectionString =
    configuration.GetConnectionString("InventoryDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'InventoryDatabase' was not found.");

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddTransient<InventoryPlugin>();

var kernel = builder.Build();

using var scope = kernel.Services.CreateScope();

var inventoryPlugin =
    scope.ServiceProvider.GetRequiredService<InventoryPlugin>();

kernel.Plugins.AddFromObject(inventoryPlugin, "Inventory");

var chat = kernel.Services.GetRequiredService<IChatCompletionService>();

var settings = new PromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();

history.AddSystemMessage("""
    You are an inventory assistant.

    Rules:
    1. When the user asks about stock using a product name,
       call GetStockByProductName.
    2. When the user explicitly provides a numeric product ID,
       call GetStockQuantity.
    3. Never invent product IDs or stock quantities.
    4. Use the Inventory plugin for actual inventory data.
    5. Distinguish hypothetical calculations from actual database data.
    6. Never claim that inventory has been changed unless a database
       operation actually performed that change.
    """);

Console.WriteLine("======================================");
Console.WriteLine("  Semantic Kernel Inventory Assistant");
Console.WriteLine("======================================");
Console.WriteLine($"Model: {ollamaModelId}");
Console.WriteLine($"Endpoint: {ollamaEndpoint}");
Console.WriteLine("Type 'exit' to quit.");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    history.AddUserMessage(input);

    try
    {
        var response = await chat.GetChatMessageContentAsync(
            history,
            executionSettings: settings,
            kernel: kernel);

        var answer = response.Content ?? string.Empty;

        Console.WriteLine($"AI: {answer}");
        history.AddAssistantMessage(answer);
    }
    catch (OperationCanceledException ex)
    {
        Console.WriteLine($"Request cancelled or timed out: {ex}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while processing your request:");
        Console.WriteLine(ex.ToString());
    }

    Console.WriteLine();
}