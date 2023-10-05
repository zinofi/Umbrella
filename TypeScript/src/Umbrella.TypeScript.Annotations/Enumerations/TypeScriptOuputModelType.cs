namespace Umbrella.TypeScript;

/// <summary>
/// The type of the model that will be generated. Flags can be used to specify multiple types.
/// </summary>
[Flags]
public enum TypeScriptOutputModelType
{
	/// <summary>
	/// A POTO Interface.
	/// </summary>
	Interface = 0b00000001,

	/// <summary>
	/// A POTO Class. A POTO Interface will also be generated.
	/// </summary>
	Class = 0b00000011, //Ensure both interface and class will be generated

	/// <summary>
	/// A Knockout Interface.
	/// </summary>
	KnockoutInterface = 0b00000100,

	/// <summary>
	/// A Knockout Class. A Knockout Interface will also be generated.
	/// </summary>
	KnockoutClass = 0b00001100, //Ensure both interface and class will be generated

	/// <summary>
	/// An Aurelia Interface.
	/// </summary>
	AureliaInterface = 0b00010000,

	/// <summary>
	/// An Aurelia Class. An Aurelia Interface will also be generated.
	/// </summary>
	AureliaClass = 0b00110000  //Ensure both interface and class will be generated
}