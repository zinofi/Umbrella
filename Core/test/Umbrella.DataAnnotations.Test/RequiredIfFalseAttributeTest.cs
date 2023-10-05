﻿using Xunit;

namespace Umbrella.DataAnnotations.Test;

public class RequiredIfFalseAttributeTest
{
	private class Model : ContingentValidationModelBase<RequiredIfFalseAttribute>
	{
		public bool? Value1 { get; set; }

		[RequiredIfFalse("Value1")]
		public string? Value2 { get; set; }
	}

	[Fact]
	public void IsValidTest()
	{
		var model = new Model() { Value1 = false, Value2 = "hello" };
		Assert.True(model.IsValid("Value2"));
	}

	[Fact]
	public void IsNotValidTest()
	{
		var model = new Model() { Value1 = false, Value2 = "" };
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
		var model = new Model() { Value1 = true, Value2 = "" };
		Assert.True(model.IsValid("Value2"));
	}

	[Fact]
	public void IsNotRequiredWithValue1NullTest()
	{
		var model = new Model() { Value1 = null, Value2 = "" };
		Assert.True(model.IsValid("Value2"));
	}
}