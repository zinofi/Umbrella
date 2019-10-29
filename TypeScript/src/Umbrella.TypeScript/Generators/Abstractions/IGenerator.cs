using System;

namespace Umbrella.TypeScript.Generators.Abstractions
{
	public interface IGenerator
	{
		TypeScriptOutputModelType OutputModelType { get; }
		string Generate(Type modelType, bool generateValidationRules, bool strictNullChecks, TypeScriptPropertyMode propertyMode);
	}
}