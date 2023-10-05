﻿using System.ComponentModel.DataAnnotations;
using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations.Test;

internal abstract class ContingentValidationModelBase<T> where T : ContingentValidationAttribute
{
	public T GetAttribute(string property) => (T)GetType().GetProperty(property)!.GetCustomAttributes(typeof(T), false).First();

	public bool IsValid(string property)
	{
		var attribute = GetAttribute(property);
		return attribute.IsValid(GetType().GetProperty(property)!.GetValue(this, null)!, this);
	}
}

internal abstract class ValidationModelBase<T> where T : ValidationAttribute
{
	public T GetAttribute(string property) => (T)GetType().GetProperty(property)!.GetCustomAttributes(typeof(T), false).First();

	public bool IsValid(string property)
	{
		var attribute = GetAttribute(property);
		return attribute.IsValid(GetType().GetProperty(property)!.GetValue(this, null));
	}
}