
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelLearning.Plugins;
using SemanticKernelLearning.Repositories;
using SemanticKernelLearning.Services;

#pragma warning disable SKEXP0070

var builder = Kernel.CreateBuilder();

builder.AddOllamaChatCompletion(
    modelId: "qwen2.5:3b",
    endpoint: new Uri("http://localhost:11434"));

builder.Services.AddSingleton<IInventoryRepository,
    InMemoryInventoryRepository>();

builder.Services.AddTransient<InventoryService>();
builder.Services.AddTransient<InventoryPlugin>();

var kernel = builder.Build();

kernel.Plugins.AddFromObject(
    ActivatorUtilities.CreateInstance<InventoryPlugin>(
        kernel.Services),
    "Inventory");

var chat = kernel.GetRequiredService<IChatCompletionService>();

var settings = new PromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();

Console.WriteLine("Inventory AI Assistant - Lesson 3");
Console.WriteLine("Ask about laptops, keyboards, or product IDs.");
Console.WriteLine("Type 'exit' to quit.");

while (true)
{
    Console.Write("\nYou: ");
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

        Console.WriteLine($"AI: {response.Content}");
        history.AddAssistantMessage(response.Content ?? "");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
