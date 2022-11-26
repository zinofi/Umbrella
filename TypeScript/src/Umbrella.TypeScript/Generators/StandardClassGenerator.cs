namespace Umbrella.TypeScript.Generators;

/// <summary>
/// A TypeScript generator implementation for generating POTO classes.
/// </summary>
/// <seealso cref="BaseClassGenerator" />
public class StandardClassGenerator : BaseClassGenerator
{
	/// <inheritdoc />
	public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.Class;

	/// <inheritdoc />
	protected override bool SupportsValidationRules => false;

	/// <inheritdoc />
	protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.Interface;
}