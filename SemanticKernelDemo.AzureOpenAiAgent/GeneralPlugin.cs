using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticKernelDemo.AzureOpenAiAgent;

public class GeneralPlugin
{
    [KernelFunction]
    [Description("Gets the current time.")]
    public TimeSpan GetTime()
    {
        return TimeProvider.System.GetLocalNow().TimeOfDay;
    }

    [KernelFunction]
    [Description("Gets the current date.")]
    public string GetDate()
    {
        return TimeProvider.System.GetLocalNow().Date.ToLongDateString();
    }
}