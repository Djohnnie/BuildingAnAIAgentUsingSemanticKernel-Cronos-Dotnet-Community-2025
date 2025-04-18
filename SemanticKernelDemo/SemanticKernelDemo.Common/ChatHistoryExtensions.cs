using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernelDemo.Common;

public static class ChatHistoryExtensions
{
    public static void Debug(this ChatHistory chatHistory)
    {
        foreach (var chat in chatHistory)
        {
            if (chat.Items != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var item in chat.Items)
                {
                    if (item is FunctionCallContent functionCall)
                    {
                        if (item.InnerContent is OpenAI.Chat.ChatToolCall toolCall)
                        {
                            Console.Write("Debug > ");
                            Console.WriteLine($"{chat.Role} > FunctionCallContent: {toolCall.FunctionName}");
                        }
                    }
                    else if (item is FunctionResultContent functionResult)
                    {
                        Console.Write("Debug > ");
                        Console.WriteLine($"{chat.Role} > FunctionResultContent: {functionResult.Result}");
                    }
                    else if (item is TextContent textContent)
                    {
                        Console.Write("Debug > ");
                        Console.WriteLine($"{chat.Role} > TextContent: {textContent.Text}");
                    }
                }
            }
        }
    }
}