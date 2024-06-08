using Microsoft.SemanticKernel;
using Kernel = Microsoft.SemanticKernel.Kernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Sentry.Protocol;
using Specklethron.Plugins;

namespace Specklethron
{
    public class Specklethron
    {
        public Kernel _kernel { get; set; }

        public Specklethron()
        {
            ILoggerFactory myLoggerFactory = NullLoggerFactory.Instance;

            var env = dotenv.net.DotEnv.Read();
            
            var kernelBuilder = Kernel.CreateBuilder()
                 .AddOpenAIChatCompletion("gpt-4-vision-preview", env["OPENAI_API_KEY"]);
                //.AddOpenAIChatCompletion("gpt-3.5-turbo", "sk-proj-5ko37rX0q0zNjat4WEI7T3BlbkFJgJqZgWdaCY4IFdGkYEhE");



            kernelBuilder.Plugins.AddFromPromptDirectory(@"SemanticKernel/Skills");
         //    kernelBuilder.Plugins.AddFromType<AgentUserInput>();

            /*
            kernelBuilder.Plugins.AddFromType<TimeCheckerPlugin>();
            kernelBuilder.Plugins.AddFromType<FileIOPluginEx>();
            */
            kernelBuilder.Plugins.AddFromType<SpeklePlugin>();
            
            _kernel = kernelBuilder.Build();
        }
       
    }
}
