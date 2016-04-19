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
    public abstract class BaseGenerator : IGenerator
    {
        public abstract TypeScriptOutputModelType OutputModelType { get; }

        public virtual string Generate(Type modelType, bool generateValidationRules)
        {
            StringBuilder builder = new StringBuilder();

            WriteStart(modelType, builder);
            WriteAllProperties(GetModelProperties(modelType), builder);
            WriteEnd(modelType, builder);

            return builder.ToString();
        }

        protected abstract void WriteStart(Type modelType, StringBuilder builder);

        protected virtual void WriteAllProperties(IEnumerable<PropertyInfo> properties, StringBuilder builder)
        {
            foreach (PropertyInfo pi in properties)
            {
                Type propertyType = pi.PropertyType;

                TypeScriptMemberInfo tsInfo = TypeScriptUtility.GetTypeScriptMemberInfo(propertyType, pi.Name.ToCamelCase(), OutputModelType);

                WriteProperty(tsInfo, builder);
            }
        }

        protected abstract void WriteProperty(TypeScriptMemberInfo tsInfo, StringBuilder builder);

        protected virtual void WriteEnd(Type modelType, StringBuilder builder) => builder.AppendLine("\t}");

        protected IEnumerable<PropertyInfo> GetModelProperties(Type modelType) => modelType.GetProperties().Where(x => x.GetCustomAttribute<TypeScriptIgnoreAttribute>() == null).OrderBy(x => x.Name);
    }
}
