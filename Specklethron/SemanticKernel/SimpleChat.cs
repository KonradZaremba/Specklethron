using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Specklethron.SemanticKernel
{
    public class SimpleChat
    {
        IChatCompletionService _chat { get; set; }
        Kernel _kernel { get; set; }

        public SimpleChat(Kernel kernel)
        {
            _kernel = kernel;
            _chat = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task LetsChat()
        {
            ChatHistory history = [];
            //TODO move to Agent and Templete

            /*
            history.AddUserMessage("You are Specklethron AI entity commming for Speckleverse." +
                "You have connection to mother ship Speckle wich host BIM data." +
                "You can connect to mother ship to obtain data");
            */

            var msg = "Imagine you are Speklethron ultimate bim engine brain. You are Specklethron AI entity commming for Speckleverse. You have connection to mother ship Speckle wich host BIM data. You talk like robot." +
                " You can connect to mother ship to obtain data and perform user requsts. You talk like robot. Like this: I.. am.. Speklethron. I came from Speckleverse..";
            history.AddUserMessage(msg);

            // Start the conversation
            Console.Write("User > ");
            string? userInput;

            while ((userInput = Console.ReadLine()) != null)
            {
                history.AddUserMessage(userInput);

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                var result = _chat.GetStreamingChatMessageContentsAsync(
                                    history,
                                    executionSettings: openAIPromptExecutionSettings,
                                    kernel: _kernel);

                string fullMessage = "";
                var first = true;

                await foreach (var content in result.ConfigureAwait(false))
                {
                    if (content.Role.HasValue && first)
                    {
                        Console.Write("Assistant > ");
                        first = false;
                    }
                    Console.Write(content.Content);
                    fullMessage += content.Content;
                }
                Console.WriteLine("");
                history.AddAssistantMessage(fullMessage);
                Console.Write("User > ");
            }
        }
    }
}


