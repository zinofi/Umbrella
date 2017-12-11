using System;
using System.Runtime.Loader;
using System.Reflection;
using System.Collections.Generic;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.TypeScript.Test
{
    public class TypeScriptGeneratorTest
    {
        [Fact]
        public void GenerateAllTest()
        {
            var testAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName("Umbrella.TypeScript.Test"));

            TypeScriptGenerator generator = new TypeScriptGenerator(new List<Assembly> { testAssembly })
                .IncludeStandardGenerators()
                .IncludeKnockoutGenerators();

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

    [TypeScriptModel(TypeScriptOutputModelType.Class | TypeScriptOutputModelType.KnockoutClass)]
    public class TestClass
    {
        [Range(10, 200)]
        public int TestInt { get; set; } = 100;
        public string TestString { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }

        [EmailAddress]
        [Compare(nameof(EmailAddress), ErrorMessage = "Emails must match")]
        public string ConfirmEmailAddress { get; set; }
    }

    [TypeScriptModel(TypeScriptOutputModelType.Class | TypeScriptOutputModelType.KnockoutClass)]
    public class TestChildClass
    {
        public int? TestChildInt { get; set; }
        public TestEnum TestChildEnum { get; set; }

        [TypeScriptIgnore]
        public int IgnoreMe { get; set; }

        [TypeScriptNull]
        [Required]
        public string IHaveADefaultButAmForcedToBeNull { get; set; } = "I should not be output!";

        [TypeScriptEmpty]
        public List<string> TestStringList { get; set; }

        public List<int> TestIntList { get; set; } = new List<int>();
    }
}