namespace Umbrella.Analyzers.Test;

public class UA001_NullCheckAnalyzerTests : AnalyzerTestBase<NullCheckAnalyzer>
{
	[Fact]
	public async Task NullEqualityComparison_ShouldTriggerDiagnostic()
	{
		const string source = @"public class TestClass { public void M(object o) { if(o == null) { } } }";
		var expected = Diagnostic(NullCheckAnalyzer.Rule, 1, 55);
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task NullInequalityComparison_ShouldTriggerDiagnostic()
	{
		const string source = @"public class TestClass { public void M(object o) { if(o != null) { } } }";
		var expected = Diagnostic(NullCheckAnalyzer.Rule, 1, 55);
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PatternMatchingNullCheck_ShouldNotTriggerDiagnostic()
	{
		const string source = @"public class TestClass { public void M(object o) { if(o is null) { } } }";
		await VerifyNoDiagnosticsAsync(source);
	}
}
