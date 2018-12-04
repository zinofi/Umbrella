using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption;

namespace Umbrella.Utilities.Benchmark.Encryption
{
    [CoreJob]
    public class PasswordGeneratorBenchmark
    {
        private readonly PasswordGenerator _passwordGenerator;

        public PasswordGeneratorBenchmark()
        {
            var logger = new Mock<ILogger<PasswordGenerator>>();
            _passwordGenerator = new PasswordGenerator(logger.Object);
        }

        [Benchmark]
        public string GeneratePassword()
        {
            return _passwordGenerator.GeneratePassword(20, 5, 5);
        }
    }
}