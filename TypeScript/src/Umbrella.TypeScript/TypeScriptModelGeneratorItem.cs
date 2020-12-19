using System;

namespace Umbrella.TypeScript
{
	/// <summary>
	/// Represents an input type used by the TypeScript generator.
	/// </summary>
	public class TypeScriptModelGeneratorItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeScriptModelGeneratorItem"/> class.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <param name="modelAttribute">The model attribute.</param>
		public TypeScriptModelGeneratorItem(Type modelType, TypeScriptModelAttribute modelAttribute)
		{
			ModelType = modelType;
			ModelAttribute = modelAttribute;
		}

		/// <summary>
		/// Gets the type of the model.
		/// </summary>
		public Type ModelType { get; }

		/// <summary>
		/// Gets the model attribute.
		/// </summary>
		public TypeScriptModelAttribute ModelAttribute { get; }
	}
}