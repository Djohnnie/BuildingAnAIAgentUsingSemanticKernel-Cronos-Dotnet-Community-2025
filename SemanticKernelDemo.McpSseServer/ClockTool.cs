using ModelContextProtocol.Server;
using System.ComponentModel;

namespace SemanticKernelDemo.McpSseServer;

[McpServerToolType]
public class ClockTool
{
    [McpServerTool]
    [Description("Gets the current time.")]
    public TimeSpan GetTime()
    {
        return TimeProvider.System.GetLocalNow().TimeOfDay;
    }

    [McpServerTool]
    [Description("Gets the current date.")]
    public string GetDate()
    {
        return TimeProvider.System.GetLocalNow().Date.ToLongDateString();
    }
}