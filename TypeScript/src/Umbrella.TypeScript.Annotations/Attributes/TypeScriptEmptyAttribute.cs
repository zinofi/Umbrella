namespace Umbrella.TypeScript;

/// <summary>
/// Used to specify that if the type of the property has been translated into a TypeScript array, e.g. <see cref="List{T}"/> has been transleted to T[],
/// and the property value on the .NET instance is initialized to null, that the value assigned to the TypeScript model should be '[]' instead of <see langword="null"/>.
/// This attribute is only respected if the TypeScriptPropertyMode is set to 'Model'.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property)]
public class TypeScriptEmptyAttribute : Attribute
{
}