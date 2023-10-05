using System.Globalization;
using System.Reflection;
using System.Text;

namespace Umbrella.TypeScript.Generators;

/// <summary>
/// Serves as the base for all interface generators.
/// </summary>
/// <seealso cref="BaseGenerator" />
public abstract class BaseInterfaceGenerator : BaseGenerator
{
	/// <inheritdoc />
	protected override bool SupportsValidationRules => false;

	/// <inheritdoc />
	protected override void WriteStart(Type modelType, StringBuilder builder)
	{
		string generatedName = TypeScriptUtility.GenerateTypeName(modelType.Name, modelType, OutputModelType);

		List<string> lstInterface = TypeScriptUtility.GetInterfaceNames(modelType, OutputModelType, false);

		_ = builder.Append($"\texport interface {generatedName}");

		if (lstInterface.Count > 0)
			_ = builder.Append(string.Format(CultureInfo.InvariantCulture, " extends {0}", string.Join(", ", lstInterface)));

		_ = builder.AppendLine();
		_ = builder.AppendLine("\t{");
	}

	/// <inheritdoc />
	protected override void WriteProperty(PropertyInfo pi, TypeScriptMemberInfo tsInfo, StringBuilder typeBuilder)
	{
		if (!string.IsNullOrEmpty(tsInfo.TypeName))
		{
			string strStrictNullCheck = StrictNullChecks && (tsInfo.IsNullable || PropertyMode == TypeScriptPropertyMode.Null) ? " | null" : "";

			_ = typeBuilder.AppendLine($"\t\t{tsInfo.Name}: {tsInfo.TypeName}{strStrictNullCheck};");
		}
	}
}