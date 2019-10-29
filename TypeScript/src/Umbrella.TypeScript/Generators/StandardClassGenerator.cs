namespace Umbrella.TypeScript.Generators
{
	public class StandardClassGenerator : BaseClassGenerator
	{
		public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.Class;
		protected override bool SupportsValidationRules => false;
		protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.Interface;
	}
}
