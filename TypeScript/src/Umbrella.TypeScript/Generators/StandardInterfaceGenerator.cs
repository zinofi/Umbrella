namespace Umbrella.TypeScript.Generators
{
	/// <summary>
	/// A TypeScript generator implementation for generating POTO interfaces.
	/// </summary>
	/// <seealso cref="Umbrella.TypeScript.Generators.BaseInterfaceGenerator" />
	public class StandardInterfaceGenerator : BaseInterfaceGenerator
	{
		/// <inheritdoc />
		public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.Interface;
	}
}