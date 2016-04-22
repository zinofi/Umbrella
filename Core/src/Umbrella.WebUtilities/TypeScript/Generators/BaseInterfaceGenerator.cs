using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.TypeScript.Attributes;
using Umbrella.WebUtilities.TypeScript.Enumerations;
using Umbrella.WebUtilities.TypeScript.Generators.Interfaces;

namespace Umbrella.WebUtilities.TypeScript.Generators
{
    public abstract class BaseInterfaceGenerator : BaseGenerator
    {
        protected override bool SupportsValidationRules => false;

        protected override void WriteStart(Type modelType, StringBuilder builder)
        {
            string generatedName = TypeScriptUtility.GenerateTypeName(modelType.Name, modelType, OutputModelType);

            List<string> lstInterface = TypeScriptUtility.GetInterfaceNames(modelType, OutputModelType, false);

            builder.Append($"\texport interface {generatedName}");

            if (lstInterface.Count > 0)
                builder.Append(string.Format(" extends {0}", string.Join(", ", lstInterface)));

            builder.AppendLine();
            builder.AppendLine("\t{");
        }

        protected override void WriteProperty(TypeScriptMemberInfo tsInfo, StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(tsInfo.TypeName))
                builder.AppendLine($"\t\t{tsInfo.Name}: {tsInfo.TypeName};");
        }
    }
}