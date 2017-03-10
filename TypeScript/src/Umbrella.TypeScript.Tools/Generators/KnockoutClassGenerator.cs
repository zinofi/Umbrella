using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.TypeScript.Tools.Generators.Interfaces;

namespace Umbrella.TypeScript.Tools.Generators
{
    public class KnockoutClassGenerator : BaseClassGenerator
    {
        public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.KnockoutClass;
        protected override bool SupportsValidationRules => false;
        protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.KnockoutInterface;

        protected override void WriteProperty(TypeScriptMemberInfo tsInfo, StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(tsInfo.TypeName))
            {
                string formatString = "\t\t{0}: ";

                if (tsInfo.TypeName.EndsWith("[]"))
                {
                    formatString += "KnockoutObservableArray<{1}> = ko.observableArray<{1}>({2});";
                    tsInfo.TypeName = tsInfo.TypeName.TrimEnd('[', ']');
                }
                else
                {
                    formatString += "KnockoutObservable<{1}> = ko.observable<{1}>({2});";
                }

                builder.AppendLine(string.Format(formatString, tsInfo.Name, tsInfo.TypeName, tsInfo.InitialOutputValue));
            }
        }
    }
}
