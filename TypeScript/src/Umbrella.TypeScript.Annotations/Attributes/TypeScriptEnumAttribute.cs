namespace Umbrella.TypeScript.Annotations.Attributes;

/// <summary>
/// Used to mark enum types to be output by the TypeScript generator.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class TypeScriptEnumAttribute : Attribute
{
}