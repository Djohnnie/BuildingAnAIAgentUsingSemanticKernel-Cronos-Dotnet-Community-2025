#pragma warning disable SKEXP0070
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.Ollama;
using SemanticKernelDemo.OllamaAgent;


var builder = Kernel.CreateBuilder();
builder.AddOllamaChatCompletion("llama3.2:3b", new Uri("http://localhost:11434/"));
builder.Plugins.AddFromType<GeneralPlugin>();

var kernel = builder.Build();

var chatCompletionAgent = new ChatCompletionAgent
{
    Name = "TimeAgent",
    Description = "Agent that knows about the current date and time.",
    Instructions = "You should only reply on questions related to the current date and time.",
    Kernel = kernel,
    Arguments = new KernelArguments(new OllamaPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    })
};

var agentThread = new ChatHistoryAgentThread();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("User > ");
    Console.ForegroundColor = ConsoleColor.White;
    var request = Console.ReadLine();
    agentThread.ChatHistory.AddUserMessage(request!);

    string fullMessage = "";
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Assistant > ");

    await foreach (var response in chatCompletionAgent.InvokeAsync(agentThread))
    {
        Console.Write(response.Message.Content);
        fullMessage += response.Message.Content;
    }

    Console.WriteLine();

    agentThread.ChatHistory.AddAssistantMessage(fullMessage);
    agentThread.ChatHistory.Debug();
}