using System;
using System.Collections.Generic;
using System.IO;
using Umbrella.TypeScript.Tools.Aurelia;
using Xunit;

namespace Umbrella.TypeScript.Tools.Test
{
    public class UmbrellaTypeScriptAureliaAppTests
    {
        [Fact]
        public void TestApp()
        {
            var app = new UmbrellaTypeScriptAureliaApp(testMode: true);

            string baseDir = AppContext.BaseDirectory;

            var args = new[]
            {
                $"--input:\"{baseDir}\"",
                $"--assemblies:\"Umbrella.TypeScript.Tools.Aurelia.Test\"",
                $"--type:\"module\"",
                "--strict",
                "--validation",
                "--property-mode:\"Model\"",
                $@"--output: ""C:\Temp\aurelia-generated.ts"""
            };

            try
            {
                int result = app.Execute(args);
            }
            catch(Exception exc)
            {
                Assert.Equal("Testing successful", exc.Message);
            }
        }
    }
}