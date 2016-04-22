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
        protected abstract bool SupportsValidationRules { get; }

        public virtual string Generate(Type modelType, bool generateValidationRules)
        {
            StringBuilder typeBuilder = new StringBuilder();

            //Only create an instance if validation rules need to be generated
            StringBuilder validationBuilder = generateValidationRules ? new StringBuilder() : null;
            
            //Write the start of the type
            WriteStart(modelType, typeBuilder);

            //Write all properties. This may or may not generate validation rules.
            WriteAllProperties(GetModelProperties(modelType), typeBuilder, validationBuilder);

            //Write the end of the type. We pass in the validationBuilder here so that the content
            //of the validationBuilder can be written to the type in a way that is specific to the generator.
            WriteEnd(modelType, typeBuilder, validationBuilder);

            return typeBuilder.ToString();
        }

        protected abstract void WriteStart(Type modelType, StringBuilder builder);

        protected virtual void WriteAllProperties(IEnumerable<PropertyInfo> properties, StringBuilder typeBuilder, StringBuilder validationBuilder)
        {
            foreach (PropertyInfo pi in properties)
            {
                Type propertyType = pi.PropertyType;

                TypeScriptMemberInfo tsInfo = TypeScriptUtility.GetTypeScriptMemberInfo(propertyType, pi.Name.ToCamelCase(), OutputModelType);
                
                WriteProperty(tsInfo, typeBuilder);

                //We are generating the validation rules here so that this work can be done in the same step
                //as the work to generate the property itself.
                if (validationBuilder != null)
                    WriteValidationRules(tsInfo, validationBuilder);
            }
        }

        protected abstract void WriteProperty(TypeScriptMemberInfo tsInfo, StringBuilder typeBuilder);

        protected virtual void WriteValidationRules(TypeScriptMemberInfo tsInfo, StringBuilder typeBuilder)
        {
            //If the generator implementation supports validation rules but they haven't been implmented
            //throw an exception to indicate this.
            if(SupportsValidationRules)
                throw new NotImplementedException("This generator has been marked as supporting validation rules but doesn't implement this method.");
        }

        protected virtual void WriteEnd(Type modelType, StringBuilder typeBuilder, StringBuilder validationBuilder) => typeBuilder.AppendLine("\t}");

        protected IEnumerable<PropertyInfo> GetModelProperties(Type modelType) => modelType.GetProperties().Where(x => x.GetCustomAttribute<TypeScriptIgnoreAttribute>() == null).OrderBy(x => x.Name);
    }
}
