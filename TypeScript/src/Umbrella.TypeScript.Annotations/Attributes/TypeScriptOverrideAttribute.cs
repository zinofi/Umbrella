using System;

namespace Umbrella.TypeScript
{
	/// <summary>
	/// This allows the type that is output in the generated TypeScript model to be different from the type in the source
	/// C# model. However, default values cannot be used when overriding types and output model properties will be initialized
	/// to null. This attribute inherits from the <see cref="TypeScriptNullAttribute"/> to accomplish this.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class TypeScriptOverrideAttribute : TypeScriptNullAttribute
	{
		/// <summary>
		/// Gets or sets the type override.
		/// </summary>
		public Type TypeOverride { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeScriptOverrideAttribute"/> class.
		/// </summary>
		/// <param name="typeOverride">The type override.</param>
		public TypeScriptOverrideAttribute(Type typeOverride)
		{
			TypeOverride = typeOverride;
		}
	}
}