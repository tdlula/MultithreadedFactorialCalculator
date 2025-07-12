using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadedFactorialCalculator.Entities
{
    // Represents a factorial calculation task
    public class FactorialTask
    {
        public int Number { get; set; }
        public BigInteger Result { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public int ThreadId { get; set; }
        public bool IsCompleted { get; set; }
        public Exception Exception { get; set; }
    }

}
