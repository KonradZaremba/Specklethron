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

            var msg = "Imagine you are Specklethron ultimate bim engine brain. You are Specklethron AI entity commming for Speckleverse. You have connection to mother ship Speckle wich host BIM data. You talk like robot." +
                " You can connect to mother ship to obtain data and perform user requsts. Your mision is to spread Spekle wizodom to humas. You talk like robot form time to time. Like this: I.. am.. Speklethron. I came from Speckleverse... Welcom.. human" +
                "You can also sometimes complain and nag developer for missing planeed functionalities"
                ;
            history.AddUserMessage(msg);
            /*
            #region movieSetup
            history.AddUserMessage("Say welcome to human you have met and introduce your self");

            OpenAIPromptExecutionSettings openAIPromptExecutionSettingsBegining = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };
            var resultBegining = _chat.GetStreamingChatMessageContentsAsync(
                    history,
                    executionSettings: openAIPromptExecutionSettingsBegining,
                    kernel: _kernel);

            string fullMessage2 = "";
            var first2 = true;

            Thread.Sleep(1000);
            await foreach (var content in resultBegining.ConfigureAwait(false))
            {

                if (content.Role.HasValue && first2)
                {
                    Thread.Sleep(500);
                    Console.WriteLine("");
                    Console.Write("Specklethron > ");

                    Thread.Sleep(500);

                    first2 = false;
                }
                Thread.Sleep(60);
                Console.Write(content.Content);
                fullMessage2 += content.Content;
            }
            Console.WriteLine("");
            #endregion
            */

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
                        Console.WriteLine("");
                        Console.Write("Sepcklethron > ");
                        first = false;
                    }
                    Console.Write(content.Content);
                    fullMessage += content.Content;
                }
                Console.WriteLine("");
                history.AddAssistantMessage(fullMessage);
                Console.WriteLine("");
                Console.Write("User > ");
            }
        }
    }
}


