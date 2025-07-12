using MultithreadedFactorialCalculator.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadedFactorialCalculator.Services
{
    // Main factorial calculator with thread management
    public class FactorialCalculator
    {
        private readonly ThreadSafeResultCollection _results = new ThreadSafeResultCollection();
        private SemaphoreSlim _semaphore;
        private int _maxThreads;
        private readonly ILogger<FactorialCalculator> _logger;

        public FactorialCalculator(int maxThreads = 0, ILogger<FactorialCalculator> logger = null)
        {
            _logger = logger;
            _maxThreads = maxThreads <= 0 ? Environment.ProcessorCount * 2 : maxThreads;
            _semaphore = new SemaphoreSlim(_maxThreads, _maxThreads);
            _logger?.LogInformation("FactorialCalculator initialized with {ThreadCount} threads", _maxThreads);
        }

        public void UpdateThreadLimit(int newLimit)
        {
            _semaphore?.Dispose();
            _maxThreads = newLimit;
            _semaphore = new SemaphoreSlim(_maxThreads, _maxThreads);
            _logger?.LogInformation("Thread limit updated to {ThreadCount}", _maxThreads);
        }

        public async Task<IEnumerable<FactorialTask>> CalculateFactorialsAsync(IEnumerable<int> numbers)
        {
            var tasks = new List<Task>();
            var numberList = numbers.ToList();

            _logger?.LogInformation("Starting calculations for {Count} numbers using max {Threads} threads", numberList.Count, _maxThreads);
            _results.PrintProgress($"Starting calculations for {numberList.Count} numbers using max {_maxThreads} threads");

            foreach (var number in numberList)
            {
                var task = CalculateFactorialAsync(number);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            return _results.GetResults();
        }

        private async Task CalculateFactorialAsync(int number)
        {
            await _semaphore.WaitAsync();

            try
            {
                var task = new FactorialTask { Number = number, ThreadId = Thread.CurrentThread.ManagedThreadId };
                var stopwatch = Stopwatch.StartNew();

                _logger?.LogDebug("Thread {ThreadId} started calculating {Number}!", task.ThreadId, number);
                _results.PrintProgress($"Thread {task.ThreadId} started calculating {number}!");

                // Perform the actual factorial calculation
                await Task.Run(() =>
                {
                    try
                    {
                        task.Result = CalculateFactorial(number);
                        task.IsCompleted = true;
                    }
                    catch (Exception ex)
                    {
                        task.Exception = ex;
                        _logger?.LogError(ex, "Error calculating factorial for {Number}", number);
                    }
                });

                stopwatch.Stop();
                task.ExecutionTime = stopwatch.Elapsed;

                _results.AddResult(task);
                _results.PrintResult(task);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private BigInteger CalculateFactorial(int n)
        {
            if (n < 0)
                throw new ArgumentException("Factorial is not defined for negative numbers");

            if (n == 0 || n == 1)
                return 1;

            BigInteger result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }

}
