namespace Umbrella.TypeScript.Generators.Abstractions;

/// <summary>
/// Specifies the behaviour of a TypeScript generator.
/// </summary>
public interface IGenerator
{
	/// <summary>
	/// Gets the type of the output model that this generator can be used to generate.
	/// </summary>
	TypeScriptOutputModelType OutputModelType { get; }

	/// <summary>
	/// Generates the TypeScript model using the specified .NET Type.
	/// </summary>
	/// <param name="modelType">Type of the model.</param>
	/// <param name="generateValidationRules">Specifies whether validation rules will be generated or not. The generated rules will depend on the <see cref="OutputModelType"/>.</param>
	/// <param name="strictNullChecks">Specifies whether strict null checks are enabled. This adds a union type of '| null' to generated properties.</param>
	/// <param name="propertyMode">The property mode.</param>
	/// <returns>The generated TypeScript model as a string.</returns>
	string Generate(Type modelType, bool generateValidationRules, bool strictNullChecks, TypeScriptPropertyMode propertyMode);
}