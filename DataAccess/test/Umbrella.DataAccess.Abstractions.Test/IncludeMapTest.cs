using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Umbrella.DataAccess.Abstractions.Test
{
	public class IncludeMapTest
	{
		[Fact]
		public void Valid()
		{
			var expressions = new Expression<Func<Type, object>>[]
			{
				x => x.Name,
				x => x.Assembly,
				x => x.Assembly.Location,
				x => x.Assembly.ImageRuntimeVersion.Length
			};

			var map = new IncludeMap<Type>(expressions);

			// Includes
			Assert.Equal(expressions.Length, map.Includes.Count);
			Assert.True(expressions.SequenceEqual(map.Includes));

			// Property Paths
			string[] propertyPaths = new[]
			{
				"Name",
				"Assembly",
				"Assembly.Location",
				"Assembly.ImageRuntimeVersion.Length"
			};

			Assert.Equal(propertyPaths.Length, map.PropertyPaths.Count);
			Assert.True(propertyPaths.SequenceEqual(map.PropertyPaths));

			// String
			Assert.Equal(string.Join(", ", propertyPaths), map.ToString());
		}
	}
}