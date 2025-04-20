using SemanticKernelDemo.McpSseServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<ClockTool>();

var app = builder.Build();

app.MapMcp();

app.Run();