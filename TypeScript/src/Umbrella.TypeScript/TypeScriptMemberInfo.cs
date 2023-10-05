namespace Umbrella.TypeScript;

/// <summary>
/// Represents the TypeScript output to generated for a specific source member, i.e. a property on a C# model.
/// </summary>
public class TypeScriptMemberInfo
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TypeScriptMemberInfo"/> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="clrType">Type of the color.</param>
	public TypeScriptMemberInfo(string name, Type clrType)
	{
		Name = name;
		CLRType = clrType;
	}

	/// <summary>
	/// Gets or sets a value indicating whether this instance is nullable.
	/// </summary>
	public bool IsNullable { get; set; }

	/// <summary>
	/// Gets the name of the member.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets or sets the name of the TypeScript type to be generated, e.g. string, number, boolean, etc.
	/// </summary>
	public string? TypeName { get; set; }

	/// <summary>
	/// Gets the CLR Type from which the TypeScript member will be generated.
	/// </summary>
	public Type CLRType { get; }

	/// <summary>
	/// Gets or sets the initial output value.
	/// </summary>
	public string? InitialOutputValue { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this instance is user defined type, e.g. another model defined
	/// by the user.
	/// </summary>
	public bool IsUserDefinedType { get; set; }
}