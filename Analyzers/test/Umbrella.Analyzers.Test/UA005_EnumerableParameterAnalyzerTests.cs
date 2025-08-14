namespace Umbrella.Analyzers.Test;

public class UA005_EnumerableParameterAnalyzerTests : AnalyzerTestBase<EnumerableParameterAnalyzer>
{
    [Fact]
    public async Task ConcreteListParameter_ShouldTriggerDiagnostic()
    {
        const string source = @"public class TestClass { public void M(List<int> items) { } }";
        var expected = Diagnostic(EnumerableParameterAnalyzer.Rule, 1, 50, "items", "List<int>");
        await VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public async Task IEnumerableParameter_ShouldNotTriggerDiagnostic()
    {
        const string source = @"public class TestClass { public void M(IEnumerable<int> items) { } }";
        await VerifyNoDiagnosticsAsync(source);
    }
}