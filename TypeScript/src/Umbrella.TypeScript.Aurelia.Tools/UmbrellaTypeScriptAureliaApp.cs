using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;
using Umbrella.TypeScript.Tools;

namespace Umbrella.TypeScript.Aurelia.Tools
{
    public class UmbrellaTypeScriptAureliaApp : UmbrellaTypeScriptApp<AureliaToolOptions>
    {
        public UmbrellaTypeScriptAureliaApp()
        {
            Name = "dotnet-umbrella-ts-aurelia";
            FullName = ".NET Core Umbrella Aurelia TypeScript Generator";
            Description = "Aurelia TypeScript generator for .NET Core applications";
        }

        protected override void SetupCommandOptions()
        {
            base.SetupCommandOptions();

            OptionDictionary.Add("validation", Option("--validation", "Specify this flag to output the import statement for the aurelia-valiation library.", CommandOptionType.NoValue));

            OptionDictionary["generators"].Description += " The Aurelia generator is included by default.";
        }

        protected override AureliaToolOptions GetToolOptions()
        {
            var toolOptions = base.GetToolOptions();

            toolOptions.ValidationEnabled = OptionDictionary["validation"].HasValue();

            return toolOptions;
        }

        protected override void SetupGenerators(List<string> generators, TypeScriptGenerator generator)
        {
            base.SetupGenerators(generators, generator);

            generator.IncludeAureliaGenerators();
        }

        protected override StringBuilder CreateOutputBuilder(AureliaToolOptions toolOptions)
        {
            StringBuilder sbOutput = base.CreateOutputBuilder(toolOptions);

            if (toolOptions.ValidationEnabled)
            {
                sbOutput.AppendLine("import { ValidationRules } from \"aurelia-validation\";");
                sbOutput.AppendLine();
            }

            return sbOutput;
        }
    }
}