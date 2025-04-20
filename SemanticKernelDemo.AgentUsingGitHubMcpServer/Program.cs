#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using SemanticKernelDemo.Common;

var endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? string.Empty;
var key = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? string.Empty;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion("gpt-4o", endpoint, key);
var kernel = builder.Build();

await using IMcpClient mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
{
    Name = "GitHub",
    Command = "npx",
    Arguments = ["-y", "modelcontextprotocol/server-github"]
}));

var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"{tool.Name}: {tool.Description}");
}

kernel.Plugins.AddFromFunctions("GitHub", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

Console.WriteLine();

var chatCompletionAgent = new ChatCompletionAgent
{
    Name = "GitHubAgent",
    Description = "Agent that helps to get information from public GitHub repositories.",
    Instructions = "You should provide information about public GitHub repositories.",
    Kernel = kernel,
    Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    })
};

var agentThread = new ChatHistoryAgentThread();

var prompt = "Summarize the last 5 commits on the Djohnnie/MijnThuis repository.";
agentThread.ChatHistory.AddUserMessage(prompt!);
await foreach (var response in chatCompletionAgent.InvokeAsync(agentThread))
{
    Console.Write(response.Message.Content);
}

Console.WriteLine();

agentThread.ChatHistory.Debug();