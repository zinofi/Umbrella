namespace Umbrella.TypeScript.Annotations.Attributes;

/// <summary>
/// Used to mark model properties that should be ignored by the TypeScript generator.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class TypeScriptIgnoreAttribute : Attribute
{
}