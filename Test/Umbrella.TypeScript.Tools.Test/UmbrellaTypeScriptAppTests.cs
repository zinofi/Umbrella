using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Umbrella.TypeScript.Tools.Test
{
    public class UmbrellaTypeScriptAppTests
    {
        [Fact]
        public void TestApp()
        {
            var app = new UmbrellaTypeScriptApp(testMode: true);

            string baseDir = AppContext.BaseDirectory;
            string outputPath = Path.Combine(baseDir, "TestApp.ts");

            var args = new[]
            {
                $"--input:{baseDir}",
                $"--assemblies:Umbrella.TypeScript.Tools.Test",
                $"--generators:standard",
                $"--generators:knockout",
                $"--type:module",
                "--strict",
                "--property-mode:Model",
                $"--output:{outputPath}"
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