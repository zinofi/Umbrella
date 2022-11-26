﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Umbrella.TypeScript.Generators;

/// <summary>
/// Serves as the base for all class generators.
/// </summary>
/// <seealso cref="BaseGenerator" />
public abstract class BaseClassGenerator : BaseGenerator
{
	/// <summary>
	/// Gets the output type of the interface model.
	/// </summary>
	protected abstract TypeScriptOutputModelType InterfaceModelType { get; }

	/// <inheritdoc />
	protected override void WriteStart(Type modelType, StringBuilder builder)
	{
		string generatedName = TypeScriptUtility.GenerateTypeName(modelType.Name, modelType, OutputModelType);

		List<string> lstInterface = TypeScriptUtility.GetInterfaceNames(modelType, InterfaceModelType, true);

		if (lstInterface.Contains(generatedName))
		{
			//List of interfaces already contains the name - strip the leading 'I' from the generatedName
			generatedName = generatedName.TrimStart('I');
		}

		builder.Append($"\texport class {generatedName}");

		if (lstInterface.Count > 0)
			builder.Append(string.Format(" implements {0}", string.Join(", ", lstInterface)));

		builder.AppendLine();
		builder.AppendLine("\t{");
	}

	/// <inheritdoc />
	protected override void WriteProperty(PropertyInfo pi, TypeScriptMemberInfo tsInfo, StringBuilder builder)
	{
		if (!string.IsNullOrEmpty(tsInfo.TypeName))
		{
			string strInitialOutputValue = PropertyMode switch
			{
				TypeScriptPropertyMode.Null => " = null",
				TypeScriptPropertyMode.Model => $" = {tsInfo.InitialOutputValue}",
				_ => "",
			};

			string strStrictNullCheck = StrictNullChecks && (tsInfo.IsNullable || PropertyMode == TypeScriptPropertyMode.Null) ? " | null" : "";

			builder.AppendLine($"\t\t{tsInfo.Name}: {tsInfo.TypeName}{strStrictNullCheck}{strInitialOutputValue};");
		}
	}
}