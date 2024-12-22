using Umbrella.TypeScript.Annotations.Enumerations;

namespace Umbrella.TypeScript.Annotations.Attributes;

/// <summary>
/// Used to mark models (classes or interfaces) to be output by the TypeScript generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false)]
public sealed class TypeScriptModelAttribute : Attribute
{
	/// <summary>
	/// The types of model that the generator should output.
	/// </summary>
	public TypeScriptOutputModelType OutputModelTypes { get; }

	/// <summary>
	/// Specifies whether the generator should output validation rules for the specified output model types.
	/// This is currently only supported for <see cref="TypeScriptOutputModelType.AureliaClass"/> models.
	/// </summary>
	public bool GenerateValidationRules { get; }

	/// <summary>
	/// The constructor for the attribute allowing the values for the
	/// <see cref="OutputModelTypes"/> and <see cref="GenerateValidationRules"/> properties
	/// to be provided as arguments.
	/// </summary>
	/// <param name="outputModelTypes">
	/// The types of model that the generator should output.
	/// </param>
	/// <param name="generateValidationRules">
	/// Specifies whether the generator should output validation rules for the specified output model types.
	/// This is currently only supported for <see cref="TypeScriptOutputModelType.AureliaClass"/> models.
	/// </param>
	public TypeScriptModelAttribute(TypeScriptOutputModelType outputModelTypes, bool generateValidationRules = true)
	{
		OutputModelTypes = outputModelTypes;
		GenerateValidationRules = generateValidationRules;
	}
}