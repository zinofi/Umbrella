﻿namespace Umbrella.Analyzers.ModelStandards.Test;

public class UMS004_CollectionsMustBeReadOnlyTests : AnalyzerTestBase
{
    [Fact]
    public async Task ListProperty_ShouldTriggerDiagnostic()
    {
        const string source = @"using System.Collections.Generic;

public record UserModel
{
    public required List<string> Tags { get; init; }
}";
        var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 5, 34, "Tags", "UserModel");
        await VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public async Task IReadOnlyCollectionProperty_ShouldNotTriggerDiagnostic()
    {
        const string source = @"using System.Collections.Generic;

public record UserModel
{
    public required IReadOnlyCollection<string> Tags { get; init; }
}";
        await VerifyNoDiagnosticsAsync(source);
    }

    [Fact]
    public async Task ListProperty_WithOptOutAttribute_ShouldNotTriggerDiagnostic()
    {
        const string source = @"using System;
using System.Collections.Generic;

public record UserModel
{
    [UmbrellaAllowMutableCollection]
    public required List<string> Tags { get; init; }
}

public class UmbrellaAllowMutableCollectionAttribute : Attribute { }";
        await VerifyNoDiagnosticsAsync(source);
    }
}
