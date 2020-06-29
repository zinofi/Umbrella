using System;

namespace Umbrella.TypeScript
{
	/// <summary>
	/// When the TypeScriptPropertyMode is set to 'Model', this is used to ensure that the property value in the generated TypeScript model is always set to <see langword="null"/>
	/// regardless of what it is initialized to in the .NET type.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Property)]
	public class TypeScriptNullAttribute : Attribute
	{
	}
}