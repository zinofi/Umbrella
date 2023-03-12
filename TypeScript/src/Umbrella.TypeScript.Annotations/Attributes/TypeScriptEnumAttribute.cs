namespace Umbrella.TypeScript;

/// <summary>
/// Used to mark enum types to be output by the TypeScript generator.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class TypeScriptEnumAttribute : Attribute
{
}