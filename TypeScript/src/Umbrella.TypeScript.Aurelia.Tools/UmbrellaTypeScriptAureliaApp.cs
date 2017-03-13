using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;
using Umbrella.TypeScript.Tools;

namespace Umbrella.TypeScript.Aurelia.Tools
{
    public class UmbrellaTypeScriptAureliaApp : UmbrellaTypeScriptApp
    {
        public UmbrellaTypeScriptAureliaApp(bool testMode = false)
            : base(testMode)
        {
        }

        protected override void SetupCommandOptions()
        {
            base.SetupCommandOptions();

            OptionDictionary.Add("validation", Option("--validation", "Specify this flag to output the import statement for the aurelia-valiation library.", CommandOptionType.NoValue));

            OptionDictionary["generators"].Description += " The Aurelia generator is included by default.";
        }

        protected override void SetupGenerators(List<string> generators, TypeScriptGenerator generator)
        {
            base.SetupGenerators(generators, generator);

            generator.IncludeAureliaGenerators();
        }

        protected override StringBuilder CreateOutputBuilder()
        {
            StringBuilder sbOutput = base.CreateOutputBuilder();

            if (OptionDictionary["validation"].HasValue())
            {
                sbOutput.AppendLine("import { ValidationRules } from \"aurelia-validation\";");
                sbOutput.AppendLine();
            }

            return sbOutput;
        }
    }
}