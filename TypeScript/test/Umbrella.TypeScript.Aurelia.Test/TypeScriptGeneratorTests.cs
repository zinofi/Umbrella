using System;
using System.Runtime.Loader;
using System.Reflection;
using System.Collections.Generic;
using Xunit;
using System.ComponentModel.DataAnnotations;
using Umbrella.DataAnnotations;

namespace Umbrella.TypeScript.Aurelia.Test
{
    public class TypeScriptGeneratorTests
    {
        [Fact]
        public void GenerateAureliaTest()
        {
            var testAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName("Umbrella.TypeScript.Aurelia.Test"));

            TypeScriptGenerator generator = new TypeScriptGenerator(new List<Assembly> { testAssembly })
                .IncludeAureliaGenerators();

            string output = generator.GenerateAll(true, true, TypeScriptPropertyMode.Model);

            Assert.NotNull(output);
        }
    }

    [TypeScriptEnum]
    public enum TestEnum
    {
        None,
        Value1,
        Value2
    }

    [TypeScriptModel(TypeScriptOutputModelType.AureliaClass)]
    public class TestClass
    {
        public int TestInt { get; set; } = 100;

        [Required]
        public string TestString { get; set; }
    }

    [TypeScriptModel(TypeScriptOutputModelType.AureliaClass)]
    public class TestChildClass
    {
        public int? TestChildInt { get; set; }
        public TestEnum TestChildEnum { get; set; }

        [TypeScriptIgnore]
        public int IgnoreMe { get; set; }

        [TypeScriptNull]
        public string IHaveADefaultButAmForcedToBeNull { get; set; } = "I should not be output!";

        [TypeScriptEmpty]
        public List<string> TestStringList { get; set; }

        [RequiredNonEmptyCollection]
        public List<int> TestIntList { get; set; } = new List<int>();
    }
}