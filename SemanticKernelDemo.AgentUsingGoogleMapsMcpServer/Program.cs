#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using SemanticKernelDemo.Common;

var endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? string.Empty;
var key = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? string.Empty;
var googleApiKey = Environment.GetEnvironmentVariable("GOOGLE_KEY") ?? string.Empty;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion("gpt-4o", endpoint, key);
var kernel = builder.Build();

await using IMcpClient mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
{
    Name = "GMaps",
    Command = "npx",
    Arguments = ["-y", "@modelcontextprotocol/server-google-maps"],
    EnvironmentVariables = new Dictionary<string, string>
    {
        { "GOOGLE_MAPS_API_KEY", googleApiKey }
    }
}));

var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
foreach (var tool in tools)
{
    Console.WriteLine($"{tool.Name}: {tool.Description}");
}

kernel.Plugins.AddFromFunctions("GMaps", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

var chatCompletionAgent = new ChatCompletionAgent
{
    Name = "GMapsAgent",
    Description = "Agent that helps to get directions using Google Maps",
    Instructions = "You should provide travel directions between two locations.",
    Kernel = kernel,
    Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    })
};

var agentThread = new ChatHistoryAgentThread();

var prompt = "Get directions between Lamot in Mechelen and Veldkant in Kontich";
agentThread.ChatHistory.AddUserMessage(prompt!);
await foreach (var response in chatCompletionAgent.InvokeAsync(agentThread))
{
    Console.Write(response.Message.Content);
}

Console.WriteLine();

agentThread.ChatHistory.Debug();