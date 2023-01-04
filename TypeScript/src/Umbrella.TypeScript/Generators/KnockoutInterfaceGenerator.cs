﻿using System.Globalization;
using System.Reflection;
using System.Text;

namespace Umbrella.TypeScript.Generators;

/// <summary>
/// A TypeScript generator implementation for generating Knockout interfaces.
/// </summary>
/// <seealso cref="BaseInterfaceGenerator" />
public class KnockoutInterfaceGenerator : BaseInterfaceGenerator
{
	private readonly bool _useDecorators;

	/// <inheritdoc />
	public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.KnockoutInterface;

	/// <summary>
	/// Initializes a new instance of the <see cref="KnockoutInterfaceGenerator"/> class.
	/// </summary>
	/// <param name="useDecorators">if set to <c>true</c>, uses TypeScript decorators.</param>
	public KnockoutInterfaceGenerator(bool useDecorators)
	{
		_useDecorators = useDecorators;
	}

	/// <inheritdoc />
	protected override void WriteProperty(PropertyInfo pi, TypeScriptMemberInfo tsInfo, StringBuilder typeBuilder)
	{
		if (!string.IsNullOrEmpty(tsInfo.TypeName))
		{
			string strStrictNullCheck = StrictNullChecks && (tsInfo.IsNullable || PropertyMode == TypeScriptPropertyMode.Null) ? " | null" : "";

			string formatString = "\t\t{0}: ";

			if (tsInfo.TypeName!.EndsWith("[]", StringComparison.Ordinal))
			{
				formatString += _useDecorators ? "{1}" : "KnockoutObservableArray<{1}>;";

				if (!_useDecorators)
					tsInfo.TypeName = tsInfo.TypeName.TrimEnd('[', ']');
			}
			else
			{
				formatString += _useDecorators ? "{1}" : "KnockoutObservable<{1}>;";
			}

			_ = typeBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, formatString, tsInfo.Name, $"{tsInfo.TypeName}{strStrictNullCheck}"));
		}
	}
}