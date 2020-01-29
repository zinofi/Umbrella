using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Xunit;

namespace Umbrella.Utilities.Test.Extensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void ToCamelCase()
        {
            Assert.Equal("capitalCity", "CapitalCity".ToCamelCase());
            Assert.Equal("movChecked", "MOVChecked".ToCamelCase());
            Assert.Equal("croatia", "Croatia".ToCamelCase());
            Assert.Equal("lowercasealready", "lowercasealready".ToCamelCase());
            Assert.Equal("alreadyCamelCase", "alreadyCamelCase".ToCamelCase());
            Assert.Equal("annualBOPAId", "AnnualBOPAId".ToCamelCase());
        }

        [Fact]
        public void ToCamelCaseInvariant()
        {
            Assert.Equal("capitalCity", "CapitalCity".ToCamelCaseInvariant());
            Assert.Equal("movChecked", "MOVChecked".ToCamelCaseInvariant());
            Assert.Equal("croatia", "Croatia".ToCamelCaseInvariant());
            Assert.Equal("lowercasealready", "lowercasealready".ToCamelCaseInvariant());
            Assert.Equal("alreadyCamelCase", "alreadyCamelCase".ToCamelCaseInvariant());
			Assert.Equal("annualBOPAId", "AnnualBOPAId".ToCamelCaseInvariant());
		}

        [Fact]
        public void ToSnakeCase()
        {
            //TODO
            //I think the method should handle existing underscores and skip them
            //i.e. if a string is already fully snake case or partially snake case then it should handle that.
            //Should these work using invariant casing rules??
            //Make them case invariant by default?
        }
    }
}