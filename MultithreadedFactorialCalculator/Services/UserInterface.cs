using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MultithreadedFactorialCalculator.Services
{
    // User interface handler
    public class UserInterface
    {
        private readonly ILogger<UserInterface> _logger;
        private FactorialCalculator _calculator;
        private int _currentThreadLimit;
        private readonly ILoggerFactory _loggerFactory;

        public UserInterface(ILogger<UserInterface> logger, ILoggerFactory loggerFactory = null)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _currentThreadLimit = Environment.ProcessorCount * 2;
            _calculator = new FactorialCalculator(_currentThreadLimit, loggerFactory?.CreateLogger<FactorialCalculator>());
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            DisplayWelcome();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation requested. Exiting...");
                    return;
                }
                DisplayMainMenu();
                var choice = GetUserChoice();

                switch (choice)
                {
                    case 1:
                        await HandleSingleFactorial();
                        break;
                    case 2:
                        await HandleBatchFactorials();
                        break;
                    case 3:
                        HandleThreadConfiguration();
                        break;
                    case 4:
                        _logger.LogInformation("Goodbye!");
                        return;
                    default:
                        _logger.LogWarning("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private void DisplayWelcome()
        {
            Console.Clear();
            Console.WriteLine("=== Multithreaded Factorial Calculator ===");
            Console.WriteLine($"System Info: {Environment.ProcessorCount} CPU cores detected");
            Console.WriteLine($"Current thread limit: {_currentThreadLimit}");
            Console.WriteLine(new string('=', 50));
            _logger.LogInformation("Application started. Thread limit: {ThreadLimit}", _currentThreadLimit);
        }

        private void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Multithreaded Factorial Calculator ===");
            Console.WriteLine($"Current thread limit: {_currentThreadLimit}");
            Console.WriteLine();
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Calculate single factorial");
            Console.WriteLine("2. Calculate multiple factorials (batch)");
            Console.WriteLine("3. Configure thread limit");
            Console.WriteLine("4. Exit");
            Console.WriteLine();
        }

        private int GetUserChoice()
        {
            Console.Write("Enter your choice (1-4): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 4)
            {
                return choice;
            }
            return -1;
        }

        private async Task HandleSingleFactorial()
        {
            Console.Write("Enter a number to calculate its factorial (1-170): ");
            if (int.TryParse(Console.ReadLine(), out int number) && number >= 1 && number <= 170)
            {
                _logger.LogInformation("Calculating factorial for {Number}", number);
                Console.WriteLine($"\nCalculating {number}!...\n");
                var results = await _calculator.CalculateFactorialsAsync(new[] { number });

                Console.WriteLine("\n" + new string('-', 50));
                Console.WriteLine("CALCULATION COMPLETE");
                Console.WriteLine(new string('-', 50));

                foreach (var result in results)
                {
                    if (result.Exception == null)
                    {
                        Console.WriteLine($"Result: {result.Number}! = {result.Result}");
                        Console.WriteLine($"Execution time: {result.ExecutionTime.TotalMilliseconds:F2}ms");
                    }
                    else
                    {
                        _logger.LogError(result.Exception, "Error calculating factorial for {Number}", result.Number);
                    }
                }
            }
            else
            {
                _logger.LogWarning("Invalid input for single factorial.");
                Console.WriteLine("Invalid input. Please enter a number between 1 and 170.");
            }
        }

        private async Task HandleBatchFactorials()
        {
            Console.WriteLine("Enter numbers separated by commas (e.g., 5,10,15,20): ");
            var input = Console.ReadLine();

            var numbers = new List<int>();
            if (!string.IsNullOrEmpty(input))
            {
                var parts = input.Split(',');
                foreach (var part in parts)
                {
                    if (int.TryParse(part.Trim(), out int number) && number >= 1 && number <= 170)
                    {
                        numbers.Add(number);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid number in batch: {Input}", part.Trim());
                        Console.WriteLine($"Invalid number: {part.Trim()}. Skipping...");
                    }
                }
            }

            if (numbers.Count > 0)
            {
                _logger.LogInformation("Calculating batch factorials for: {Numbers}", string.Join(", ", numbers));
                Console.WriteLine($"\nCalculating factorials for: {string.Join(", ", numbers)}\n");
                var stopwatch = Stopwatch.StartNew();
                var results = await _calculator.CalculateFactorialsAsync(numbers);
                stopwatch.Stop();

                Console.WriteLine("\n" + new string('-', 50));
                Console.WriteLine("BATCH CALCULATION COMPLETE");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"Total execution time: {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
                Console.WriteLine($"Numbers processed: {results.Count()}");
                Console.WriteLine($"Average time per calculation: {stopwatch.Elapsed.TotalMilliseconds / results.Count():F2}ms");

                var successful = results.Count(r => r.Exception == null);
                var failed = results.Count(r => r.Exception != null);
                Console.WriteLine($"Successful: {successful}, Failed: {failed}");
            }
            else
            {
                _logger.LogWarning("No valid numbers entered for batch factorials.");
                Console.WriteLine("No valid numbers entered.");
            }
        }

        private void HandleThreadConfiguration()
        {
            Console.WriteLine($"Current thread limit: {_currentThreadLimit}");
            Console.WriteLine($"System has {Environment.ProcessorCount} CPU cores");
            Console.WriteLine($"Recommended range: 1 to {Environment.ProcessorCount * 4}");
            Console.Write("Enter new thread limit: ");

            if (int.TryParse(Console.ReadLine(), out int newLimit) && newLimit >= 1 && newLimit <= Environment.ProcessorCount * 8)
            {
                _currentThreadLimit = newLimit;
                _calculator.UpdateThreadLimit(_currentThreadLimit);
                _logger.LogInformation("Thread limit updated to {ThreadLimit}", _currentThreadLimit);
                Console.WriteLine($"Thread limit updated to {_currentThreadLimit}");
            }
            else
            {
                _logger.LogWarning("Invalid thread limit entered: {ThreadLimit}", newLimit);
                Console.WriteLine("Invalid thread limit. Please enter a number between 1 and " + (Environment.ProcessorCount * 8));
            }
        }

        public void Dispose()
        {
            _calculator?.Dispose();
        }
    }
}
