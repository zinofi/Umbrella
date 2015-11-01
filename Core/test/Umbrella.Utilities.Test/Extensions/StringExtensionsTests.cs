using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Xunit;

namespace TRW.AftermarketMVC.Utilities.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToCamelCaseValid()
        {
            Assert.Equal("capitalCity", "CapitalCity".ToCamelCase());
            Assert.Equal("movChecked", "MOVChecked".ToCamelCase());
            Assert.Equal("croatia", "Croatia".ToCamelCase());
            Assert.Equal("lowercasealready", "lowercasealready".ToCamelCase());
            Assert.Equal("alreadyCamelCase", "alreadyCamelCase".ToCamelCase());
        }
    }
}
