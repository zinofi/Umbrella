using System.Reflection;
using System.Text;

namespace Umbrella.TypeScript.Generators
{
	public class KnockoutInterfaceGenerator : BaseInterfaceGenerator
	{
		private readonly bool _useDecorators;
		public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.KnockoutInterface;

		public KnockoutInterfaceGenerator(bool useDecorators)
		{
			_useDecorators = useDecorators;
		}

		protected override void WriteProperty(PropertyInfo pi, TypeScriptMemberInfo tsInfo, StringBuilder builder)
		{
			if (!string.IsNullOrEmpty(tsInfo.TypeName))
			{
				string strStrictNullCheck = StrictNullChecks && (tsInfo.IsNullable || PropertyMode == TypeScriptPropertyMode.Null) ? " | null" : "";

				string formatString = "\t\t{0}: ";

				if (tsInfo.TypeName.EndsWith("[]"))
				{
					formatString += _useDecorators ? "{1}" : "KnockoutObservableArray<{1}>;";

					if (!_useDecorators)
						tsInfo.TypeName = tsInfo.TypeName.TrimEnd('[', ']');
				}
				else
				{
					formatString += _useDecorators ? "{1}" : "KnockoutObservable<{1}>;";
				}

				builder.AppendLine(string.Format(formatString, tsInfo.Name, $"{tsInfo.TypeName}{strStrictNullCheck}"));
			}
		}
	}
}