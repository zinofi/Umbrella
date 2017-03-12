using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Umbrella.Utilities;
using System.Linq;
using Newtonsoft.Json;

namespace Umbrella.TypeScript.Tools
{
    public class UmbrellaTypeScriptApp : CommandLineApplication
    {
        protected static ConsoleColor s_InitialConsoleColor;

        public UmbrellaTypeScriptApp(bool testMode = false)
        {
            //Store the initial Console text color so we can reset it if we alter it at some point.
            s_InitialConsoleColor = Console.ForegroundColor;

            Name = "dotnet-umbrella-ts";
            FullName = ".NET Core Umbrella TypeScript Generator";
            Description = "TypeScript generator for .NET Core applications";

            HelpOption("-?|-h|--help");

            Dictionary<string, CommandOption> dicOption = SetupCommandOptions();

            OnExecute(() =>
            {
                bool verbose = dicOption["verbose"].HasValue();
                bool debug = dicOption["debug"].HasValue();
                string assemblyFolderPath = dicOption["input"].Value()?.Trim('"');
                List<string> assemblyNames = dicOption["assemblies"].Values?.Select(x => x.Trim('"')).ToList();
                List<string> generators = dicOption["generators"].Values?.Select(x => x.Trim('"')).ToList();
                string outputType = dicOption["type"].Value()?.Trim('"');
                bool strictNullChecks = dicOption["strict"].HasValue();
                string propertyMode = dicOption["property-mode"].Value()?.Trim('"');
                string outputPath = dicOption["output"].Value()?.Trim('"');

                if (debug)
                {
                    var parsedOptions = new
                    {
                        verbose,
                        debug,
                        assemblyFolderPath,
                        assemblyNames,
                        generators,
                        outputType,
                        strictNullChecks,
                        propertyMode,
                        outputPath
                    };

                    WriteConsoleDebugMessage($"Parsed options: {JsonConvert.SerializeObject(parsedOptions)}");
                }

                Guard.ArgumentNotNullOrWhiteSpace(assemblyFolderPath, "--assemblies|-a");
                Guard.ArgumentNotNull(generators, "--generators|-g");
                Guard.ArgumentNotNullOrWhiteSpace(outputType, "--type|-t");
                Guard.ArgumentNotNullOrWhiteSpace(propertyMode, "--property-mode|-p");
                Guard.ArgumentNotNullOrWhiteSpace(outputPath, "--output|-o");

                if (!Enum.TryParse(propertyMode, out TypeScriptPropertyMode tsPropertyMode))
                {
                    WriteConsoleErrorMessage($"The value for the --property-mode|-p argument {propertyMode} is invalid.");
                    return 3;
                }

                //Check folder exists
                if (!Directory.Exists(assemblyFolderPath))
                {
                    WriteConsoleErrorMessage($"The path for the --input|-i argument {assemblyFolderPath} does not exist.");
                    return 3;
                }

                List<string> lstAssemblyName = new List<string>();

                foreach (var fileName in Directory.EnumerateFiles(assemblyFolderPath, "*.dll"))
                {
                    if (assemblyNames.Count > 0)
                    {
                        if (!assemblyNames.Contains(Path.GetFileNameWithoutExtension(fileName)))
                            continue;
                    }

                    lstAssemblyName.Add(fileName);
                }

                if (lstAssemblyName.Count == 0)
                {
                    WriteConsoleErrorMessage($"No assemblies were found to process.");
                    return 3;
                }

                if (testMode)
                    throw new Exception("Testing successful");

                List<Assembly> lstAssemblyToProcess = new List<Assembly>();

                foreach (string assemblyName in lstAssemblyName)
                {
                    using (FileStream fs = File.OpenRead(assemblyName))
                    {
                        Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(fs);

                        lstAssemblyToProcess.Add(assembly);
                    }
                }

                if (lstAssemblyToProcess.Count == 0)
                {
                    WriteConsoleErrorMessage($"No assemblies were loaded to process.");
                    return 3;
                }

                var generator = new TypeScriptGenerator(lstAssemblyToProcess);

                SetupGenerators(generators, generator);

                string strOutput = generator.GenerateAll(outputType == "module", strictNullChecks, tsPropertyMode);
                StringBuilder sbOutput = CreateOutputBuilder();

                sbOutput.AppendLine(strOutput);

                using (StreamWriter sw = File.CreateText(outputPath))
                {
                    sw.Write(sbOutput.ToString());
                }

                return 0;
            });
        }

        protected virtual StringBuilder CreateOutputBuilder()
        {
            return new StringBuilder()
                .AppendLine("//------------------------------------------------------------------------------")
                .AppendLine("// <auto-generated>")
                .AppendLine("//")
                .AppendLine("// This code has been automatically generated by a tool. Any changes made to")
                .AppendLine("// this file will be overwritten the next time the tool is run.")
                .AppendLine("//")
                .AppendLine("// </auto-generated>")
                .AppendLine("//------------------------------------------------------------------------------");
        }

        protected virtual void SetupGenerators(List<string> generators, TypeScriptGenerator generator)
        {
            if (generators.Contains("standard"))
                generator.IncludeStandardGenerators();

            if (generators.Contains("knockout"))
                generator.IncludeKnockoutGenerators();
        }

        protected virtual Dictionary<string, CommandOption> SetupCommandOptions()
        {
            CommandOption coVerbose = Option("--verbose|-v", "Show detailed output messages.", CommandOptionType.NoValue);
            CommandOption coDebug = Option("--debug|-d", "Show debug messages.", CommandOptionType.NoValue);

            CommandOption coAssemblyFolderPath = Option("--input|-i", "The physical path to the folder containing the assemblies to scan for TypeScript attributes.", CommandOptionType.SingleValue);
            CommandOption coAssemblyNames = Option("--assemblies|-a", "The names of the assemblies to scan for attributes. If not supplied all assemblies in the folder path will be scanned.", CommandOptionType.MultipleValue);
            CommandOption coGenerators = Option("--generators|-g", "The generators to include: [standard | knockout]", CommandOptionType.MultipleValue);
            CommandOption coOutputType = Option("--type|-t", "The output type: [namespace, module]", CommandOptionType.SingleValue);
            CommandOption coStrictNullChecks = Option("--strict|-s", "Enable strict null checks", CommandOptionType.NoValue);
            CommandOption coPropertyMode = Option("--property-mode|-p", "The TypeScriptPropertyMode to use: [none, null, model]", CommandOptionType.SingleValue);
            CommandOption coOutputPath = Option("--output|-o", "The path where the output file will be written.", CommandOptionType.SingleValue);
            
            return new Dictionary<string, CommandOption>
            {
                ["verbose"] = coVerbose,
                ["debug"] = coDebug,
                ["input"] = coAssemblyFolderPath,
                ["assemblies"] = coAssemblyNames,
                ["generators"] = coGenerators,
                ["type"] = coOutputType,
                ["strict"] = coStrictNullChecks,
                ["property-mode"] = coPropertyMode,
                ["output"] = coOutputPath
            };
        }

        public void WriteConsoleErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ForegroundColor = s_InitialConsoleColor;
        }

        public void WriteConsoleDebugMessage(string message) => WriteColoredConsoleMessage(message, ConsoleColor.Yellow);

        public void WriteConsoleInfoMessage(string message) => WriteColoredConsoleMessage(message, ConsoleColor.Cyan);

        private void WriteColoredConsoleMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = s_InitialConsoleColor;
        }
    }
}