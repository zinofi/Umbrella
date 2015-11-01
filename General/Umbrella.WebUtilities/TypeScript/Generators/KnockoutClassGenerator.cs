using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.TypeScript.Enumerations;
using Umbrella.WebUtilities.TypeScript.Generators.Interfaces;

namespace Umbrella.WebUtilities.TypeScript.Generators
{
    public class KnockoutClassGenerator : IGenerator
    {
        public TypeScriptOutputModelType OutputModelType
        {
            get { return TypeScriptOutputModelType.KnockoutClass; }
        }

        public string Generate(Type modelType)
        {
            string generatedName = TypeScriptUtility.GenerateTypeName(modelType.Name, modelType, OutputModelType);

            List<string> lstInterface = TypeScriptUtility.GetInterfaceNames(modelType, TypeScriptOutputModelType.KnockoutInterface, true);

            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("\texport class {0}", generatedName));

            if (lstInterface.Count > 0)
                builder.Append(string.Format(" implements {0}", string.Join(", ", lstInterface)));

            builder.AppendLine();
            builder.AppendLine("\t{");

            foreach (PropertyInfo pi in modelType.GetProperties().OrderBy(x => x.Name))
            {
                Type propertyType = pi.PropertyType;

                TypeScriptMemberInfo tsInfo = TypeScriptUtility.GetTypeScriptMemberInfo(propertyType, pi.Name.ToCamelCase(), OutputModelType);

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

            builder.AppendLine("\t}");
            return builder.ToString();
        }
    }
}
