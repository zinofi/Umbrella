using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Options;

namespace Umbrella.Utilities.Benchmark.Encryption
{
    [CoreJob, ClrJob]
    [MemoryDiagnoser]
    public class SecureStringGeneratorBenchmark
    {
        private readonly SecureRandomStringGenerator _secureStringGenerator;

        public SecureStringGeneratorBenchmark()
        {
            var logger = new Mock<ILogger<SecureRandomStringGenerator>>();
			var options = new SecureRandomStringGeneratorOptions();

            _secureStringGenerator = new SecureRandomStringGenerator(logger.Object, options);
        }

        [Benchmark]
        public string Generate()
        {
            return _secureStringGenerator.Generate(20, 5, 5);
        }

#if !AzureDevOps
        [Benchmark(Baseline = true)]
        public string GenerateOld()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            return _secureStringGenerator.GenerateOld(20, 5, 5);
#pragma warning restore CS0612 // Type or member is obsolete
        }
#endif
    }
}