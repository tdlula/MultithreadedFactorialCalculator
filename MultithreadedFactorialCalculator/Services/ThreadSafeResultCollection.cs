using MultithreadedFactorialCalculator.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadedFactorialCalculator.Services
{
    // Thread-safe result collection
    public class ThreadSafeResultCollection
    {
        private readonly ConcurrentDictionary<int, FactorialTask> _results = new ConcurrentDictionary<int, FactorialTask>();
        private readonly object _consoleLock = new object();

        public void AddResult(FactorialTask task)
        {
            _results.TryAdd(task.Number, task);
        }

        public IEnumerable<FactorialTask> GetResults()
        {
            return _results.Values.OrderBy(t => t.Number);
        }

        public void PrintProgress(string message)
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
        }

        public void PrintResult(FactorialTask task)
        {
            lock (_consoleLock)
            {
                if (task.Exception != null)
                {
                    Console.WriteLine($"❌ {task.Number}! - Error: {task.Exception.Message} (Thread: {task.ThreadId})");
                }
                else
                {
                    Console.WriteLine($"✅ {task.Number}! = {task.Result} (Time: {task.ExecutionTime.TotalMilliseconds:F2}ms, Thread: {task.ThreadId})");
                }
            }
        }
    }
}
