using appWebSocketChat.Client;
using appWebSocketChat.Common;
using appWebSocketChat.Common.CustomExceptions;
using appWebSocketChat.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace appWebSocketChat
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(opt => 
                {
                    opt.AddConsole();
                })
                .Configure<LoggerFilterOptions>(opt => opt.MinLevel = LogLevel.Debug)
                .BuildServiceProvider();

            var logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogInformation("Starting application");


            // Get port number from arguments.
            string port = args == null || args.Length <= 0 ? string.Empty : args[0];

            logger.LogInformation($"Indicated port: {port}");

            try
            {
                Validator.ValidatePortNumber(port);

                // Tries to launch client.
                new ConsoleClientHandler(logger, int.Parse(port)).Start().Wait();
            }
            catch (InvalidPortException ipex)
            {
                logger.LogError($"Invalid port exception: {ipex.Message}");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            catch (AggregateException aex)
            {
                aex.Handle(ex =>
                {
                    // If not server found, launch server.
                    if (ex is WebSocketServerNotFoundException)
                        new ConsoleServerHandler(logger, int.Parse(port)).Start();

                    return
                        ex is WebSocketServerNotFoundException;
                });
            }
        }
    }
}
