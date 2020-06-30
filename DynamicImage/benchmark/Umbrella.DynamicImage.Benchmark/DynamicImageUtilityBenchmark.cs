using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.DynamicImage.Benchmark
{
	[MemoryDiagnoser]
	[BenchmarkCategory(nameof(DynamicImageUtility))]
	[SimpleJob(RuntimeMoniker.Net461), SimpleJob(RuntimeMoniker.NetCoreApp31)]
	public class DynamicImageUtilityBenchmark
	{
		private readonly DynamicImageUtility m_DynamicImageUtility;

		public DynamicImageUtilityBenchmark()
		{
			var logger = new Mock<ILogger<DynamicImageUtility>>();
			m_DynamicImageUtility = new DynamicImageUtility(logger.Object);
		}

		[Benchmark]
		public DynamicImageOptions TryParseUrl()
		{
			var (_, imageOptions) = m_DynamicImageUtility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.jpg");

			return imageOptions;
		}
	}
}