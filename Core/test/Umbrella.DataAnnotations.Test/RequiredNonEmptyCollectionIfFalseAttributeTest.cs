using System.Collections.Generic;
using Xunit;

namespace Umbrella.DataAnnotations.Test
{
	public class RequiredNonEmptyCollectionIfFalseAttributeTest
	{
		private class Model : ContingentValidationModelBase<RequiredNonEmptyCollectionIfFalseAttribute>
		{
			public bool? Value1 { get; set; }

			[RequiredNonEmptyCollectionIfFalse("Value1")]
			public List<string>? Value2 { get; set; }
		}

		[Fact]
		public void IsValidTest()
		{
			var model = new Model() { Value1 = false, Value2 = new List<string> { "hello" } };
			Assert.True(model.IsValid("Value2"));
		}

		[Fact]
		public void IsNotValidTest()
		{
			var model = new Model() { Value1 = false, Value2 = new List<string>() };
			Assert.False(model.IsValid("Value2"));
		}

		[Fact]
		public void IsNotValidWithValue2NullTest()
		{
			var model = new Model() { Value1 = false, Value2 = null };
			Assert.False(model.IsValid("Value2"));
		}

		[Fact]
		public void IsNotRequiredTest()
		{
			var model = new Model() { Value1 = true, Value2 = null };
			Assert.True(model.IsValid("Value2"));
		}

		[Fact]
		public void IsNotRequiredWithValue1NullTest()
		{
			var model = new Model() { Value1 = null, Value2 = null };
			Assert.True(model.IsValid("Value2"));
		}
	}
}