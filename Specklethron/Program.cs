using Serilog.Events;
using Speckle.Core.Logging;
using Specklethron;
using Specklethron.SemanticKernel;
using dotenv.net;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //Setup
        Setup.Init("v1", "myApp", new SpeckleLogConfiguration(LogEventLevel.Error, false, false, false, false, false));
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Title = "Chat";

        DotEnv.Load();

        var specklethron = new Specklethron.Specklethron();
        var chat = new SimpleChat(specklethron._kernel);

        

        await chat.LetsChat();
    }
}