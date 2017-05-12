﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.DynamicImage.Benchmark
{
    [CoreJob, ClrJob]
    public class DynamicImageUtilityBenchmark
    {
        private readonly IDynamicImageUtility m_DynamicImageUtility;

        public DynamicImageUtilityBenchmark()
        {
            var logger = new Mock<ILogger<DynamicImageUtility>>();
            m_DynamicImageUtility = new DynamicImageUtility(logger.Object);
        }

        [Benchmark]
        public DynamicImageOptions TryParseUrl()
        {
            var result = m_DynamicImageUtility.TryParseUrl("dynamicimage", "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.jpg");

            return result.ImageOptions;
        }
    }
}