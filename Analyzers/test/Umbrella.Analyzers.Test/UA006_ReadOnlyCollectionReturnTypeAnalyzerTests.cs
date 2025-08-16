namespace Umbrella.Analyzers.Test;

public class UA006_ReadOnlyCollectionReturnTypeAnalyzerTests : AnalyzerTestBase<ReadOnlyCollectionReturnTypeAnalyzer>
{
    [Fact]
    public async Task ListReturnType_ShouldTriggerDiagnostic()
    {
        const string source = @"public class TestClass { public List<int> M() { return new List<int>(); } }";
        var expected = Diagnostic(ReadOnlyCollectionReturnTypeAnalyzer.Rule, 1, 43, "List<int>");
        await VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public async Task IReadOnlyCollectionReturnType_ShouldNotTriggerDiagnostic()
    {
        const string source = @"public class TestClass { public IReadOnlyCollection<int> M() { return new List<int>(); } }";
        await VerifyNoDiagnosticsAsync(source);
    }

    [Fact]
    public async Task TupleWithList_ShouldTriggerDiagnostic()
    {
        const string source = @"public class TestClass { public (List<int>, int) M() { return (new List<int>(), 0); } }";
        var expected = Diagnostic(ReadOnlyCollectionReturnTypeAnalyzer.Rule, 1, 50, "tuple");
        await VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public async Task TupleWithIReadOnlyCollection_ShouldNotTriggerDiagnostic()
    {
        const string source = @"public class TestClass { public (IReadOnlyCollection<int>, int) M() { return (new List<int>(), 0); } }";
        await VerifyNoDiagnosticsAsync(source);
    }

    [Fact]
    public async Task SystemTupleWithList_ShouldTriggerDiagnostic()
    {
        const string source = @"using System; public class TestClass { public Tuple<List<int>, int> M() { return Tuple.Create(new List<int>(), 0); } }";
        var expected = Diagnostic(ReadOnlyCollectionReturnTypeAnalyzer.Rule, 1, 69, "System.Tuple<List<int>, int>");
        await VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public async Task SystemTupleWithIReadOnlyCollection_ShouldNotTriggerDiagnostic()
    {
        const string source = @"using System; public class TestClass { public Tuple<IReadOnlyCollection<int>, int> M() { return Tuple.Create<IReadOnlyCollection<int>, int>(new List<int>(), 0); } }";
        await VerifyNoDiagnosticsAsync(source);
    }

    [Fact]
    public async Task TaskWithList_ShouldTriggerDiagnostic()
    {
        const string source = @"using System.Threading.Tasks; public class TestClass { public Task<List<int>> M() { return Task.FromResult(new List<int>()); } }";
        var expected = Diagnostic(ReadOnlyCollectionReturnTypeAnalyzer.Rule, 1, 55, "List<int>");
        await VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public async Task TaskWithIReadOnlyCollection_ShouldNotTriggerDiagnostic()
    {
        const string source = @"using System.Threading.Tasks; public class TestClass { public Task<IReadOnlyCollection<int>> M() { return Task.FromResult<IReadOnlyCollection<int>>(new List<int>()); } }";
        await VerifyNoDiagnosticsAsync(source);
    }

    [Fact]
    public async Task ValueTaskWithList_ShouldTriggerDiagnostic()
    {
        const string source = @"using System.Threading.Tasks; public class TestClass { public ValueTask<List<int>> M() { return new ValueTask<List<int>>(new List<int>()); } }";
        var expected = Diagnostic(ReadOnlyCollectionReturnTypeAnalyzer.Rule, 1, 84, "List<int>");
        await VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public async Task ValueTaskWithIReadOnlyCollection_ShouldNotTriggerDiagnostic()
    {
        const string source = @"using System.Threading.Tasks; public class TestClass { public ValueTask<IReadOnlyCollection<int>> M() { return new ValueTask<IReadOnlyCollection<int>>(new List<int>()); } }";
        await VerifyNoDiagnosticsAsync(source);
    }
}