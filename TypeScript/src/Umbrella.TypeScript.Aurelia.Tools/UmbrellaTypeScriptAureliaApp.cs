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

            OptionDictionary["generators"].Description += " The Aurelia generator is included by default.";
        }

        protected override void SetupGenerators(TypeScriptGenerator generator, AureliaToolOptions options)
        {
            generator.IncludeAureliaGenerators();

            base.SetupGenerators(generator, options);
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