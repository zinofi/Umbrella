namespace Umbrella.TypeScript.Annotations.Attributes;

/// <summary>
/// When the TypeScriptPropertyMode is set to 'Model', this is used to ensure that the property value in the generated TypeScript model is always set to <see langword="null"/>
/// regardless of what it is initialized to in the .NET type.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property)]
#pragma warning disable CA1813 // Avoid unsealed attributes
public class TypeScriptNullAttribute : Attribute
#pragma warning restore CA1813 // Avoid unsealed attributes
{
}