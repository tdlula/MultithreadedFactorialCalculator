using MultithreadedFactorialCalculator.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Threading;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up the logger factory for console logging
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });
        ILogger logger = loggerFactory.CreateLogger<Program>();

        // Set up cancellation support for graceful shutdown (Ctrl+C)
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nShutting down gracefully..."); // Inform user on console
            logger.LogInformation("Shutting down gracefully..."); // Log shutdown event
            cts.Cancel(); // Signal cancellation to the app
            e.Cancel = true; // Prevent immediate process termination
        };

        // Create the user interface, passing in the logger and logger factory
        var ui = new UserInterface(loggerFactory.CreateLogger<UserInterface>(), loggerFactory);
        try
        {
            // Run the main UI loop, passing the cancellation token
            await ui.RunAsync(cts.Token);
        }
        catch (Exception ex)
        {
            // Log and display any unhandled exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
            logger.LogError(ex, "An error occurred");
        }
        finally
        {
            // Ensure resources are cleaned up
            ui.Dispose();
        }
    }
}