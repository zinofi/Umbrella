using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbrella.TypeScript.Generators;
using Umbrella.TypeScript.Generators.Interfaces;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Comparers;

namespace Umbrella.TypeScript
{
    //This generator does not currently handle non-user types that are not marked with the TypeScriptModelAttribute
    //i.e. a type that is part of the .NET framework other than a primitive, DateTime or string, array or IEnumerable
    public class TypeScriptGenerator
    {
        #region Private Members
        private readonly List<Type> m_Types;
        #endregion

        #region Public Properties
        public HashSet<IGenerator> Generators { get; } = new HashSet<IGenerator>(new GenericEqualityComparer<IGenerator>(x => x.GetType()));
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a new <see cref="TypeScriptGenerator"/> instance.
        /// </summary>
        /// <param name="onlyNamedAssemblies">
        /// A list of assembly names to scan for <see cref="TypeScriptModelAttribute"/> declarations.
        /// If no names are specified then all assemblies in the current <see cref="AppDomain"/> will be loaded
        /// and scanned.
        /// </param>
        public TypeScriptGenerator(params string[] onlyNamedAssemblies)
        {
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (onlyNamedAssemblies?.Length > 0)
                assemblies = assemblies.Where(x => onlyNamedAssemblies.Contains(x.GetName().Name));

            m_Types = assemblies.SelectMany(a => a.GetTypes()).ToList();
        }

        public TypeScriptGenerator IncludeStandardGenerators()
        {
            Generators.Add(new StandardInterfaceGenerator());
            Generators.Add(new StandardClassGenerator());

            return this;
        }

        public TypeScriptGenerator IncludeKnockoutGenerators()
        {
            Generators.Add(new KnockoutInterfaceGenerator());
            Generators.Add(new KnockoutClassGenerator());

            return this;
        }

        public string GenerateAll(bool outputAsModuleExport)
        {
            StringBuilder sbNamespaces = new StringBuilder();

            //Before processing the models, firstly find all the enums that need to be generated
            Dictionary<string, List<Type>> enumGroups = GetEnumItems().GroupBy(x => x.Namespace).ToDictionary(x => x.Key, x => x.ToList());

            //Generate the models
            foreach(var group in GetModelItems().GroupBy(x => x.ModelType.Namespace))
            {
                string nsName = group.Key;

                //Start of TypeScript namespace or module export
                string namespaceOrModuleStart = outputAsModuleExport
                    ? "export module"
                    : "namespace";

                sbNamespaces.AppendLine($"{namespaceOrModuleStart} {nsName}")
                    .AppendLine("{");

                //Generate enum definitions for this namespace if any exist
                if(enumGroups.ContainsKey(nsName))
                {
                    List<Type> lstEnumToGenerate = enumGroups[nsName];
                    
                    foreach(Type enumType in lstEnumToGenerate)
                    {
                        string enumOutput = GenerateEnumDefinition(enumType);

                        sbNamespaces.AppendLine(enumOutput);
                    }

                    //Remove this key from the dictionary seeing as it has now been processed
                    enumGroups.Remove(nsName);
                }

                //Generate model interfaces and classes
                foreach (TypeScriptModelGeneratorItem item in group)
                {
                    //Generate the models using the registered generators
                    foreach (IGenerator generator in Generators)
                    {
                        TypeScriptModelAttribute attribute = item.ModelAttribute;

                        if(attribute.OutputModelTypes.HasFlag(generator.OutputModelType))
                        {
                            string generatorOutput = generator.Generate(item.ModelType, attribute.GenerateValidationRules);

                            sbNamespaces.AppendLine(generatorOutput);
                        }
                    }
                }

                //End of TypeScript namespace
                sbNamespaces.AppendLine("}");
            }

            //Now generate enums in namespaces that couldn't be placed within the same namespace as any of the generated models
            foreach(var group in enumGroups)
            {
                //Start of TypeScript namespace
                sbNamespaces.AppendLine("namespace " + group.Key)
                    .AppendLine("{");

                foreach (Type enumType in group.Value)
                {
                    string enumOutput = GenerateEnumDefinition(enumType);

                    sbNamespaces.AppendLine(enumOutput);
                }

                //End of TypeScript namespace
                sbNamespaces.AppendLine("}");
            }

            return sbNamespaces.ToString();
        }
        #endregion

        #region Private Methods
        private string GenerateEnumDefinition(Type enumType)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("\texport enum {0}", enumType.Name));
            builder.AppendLine("\t{");

            foreach(var enumItem in enumType.GetEnumDictionary())
            {
                builder.AppendLine(string.Format("\t\t{0} = {1},", enumItem.Value, enumItem.Key));
            }

            builder.AppendLine("\t}");
            return builder.ToString();
        }

        private IEnumerable<TypeScriptModelGeneratorItem> GetModelItems()
        {
            foreach (Type type in m_Types)
            {
                TypeScriptModelAttribute modelAttribute = type.GetCustomAttribute<TypeScriptModelAttribute>();

                if (modelAttribute == null)
                    continue;

                yield return new TypeScriptModelGeneratorItem { ModelType = type, ModelAttribute = modelAttribute };
            }
        }

        private IEnumerable<Type> GetEnumItems()
        {
            foreach(Type type in m_Types)
            {
                if (!type.IsEnum)
                    continue;

                TypeScriptEnumAttribute enumAttribute = type.GetCustomAttribute<TypeScriptEnumAttribute>();

                if (enumAttribute == null)
                    continue;

                yield return type;
            }
        }
        #endregion
    }
}