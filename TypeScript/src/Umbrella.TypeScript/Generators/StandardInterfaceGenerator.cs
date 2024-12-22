using Umbrella.TypeScript.Annotations.Enumerations;

namespace Umbrella.TypeScript.Generators;

/// <summary>
/// A TypeScript generator implementation for generating POTO interfaces.
/// </summary>
/// <seealso cref="BaseInterfaceGenerator" />
public class StandardInterfaceGenerator : BaseInterfaceGenerator
{
	/// <inheritdoc />
	public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.Interface;
}