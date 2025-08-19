namespace Umbrella.Analyzers.Test;

public class UA010_PrimaryConstructorUsageAnalyzerTests : AnalyzerTestBase<PrimaryConstructorUsageAnalyzer>
{
	[Fact]
	public async Task PrimaryConstructor_Class_ShouldTriggerDiagnostic()
	{
		const string source = @"class C(int x) { }";
		// 'C' starts at column 7: 1 2 3 4 5 6 7
		var expected = Diagnostic(PrimaryConstructorUsageAnalyzer.Rule, 1, 7, "C");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PrimaryConstructor_Struct_ShouldTriggerDiagnostic()
	{
		const string source = @"struct S(int x) { }";
		// 'S' at column 8.
		var expected = Diagnostic(PrimaryConstructorUsageAnalyzer.Rule, 1, 8, "S");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task RegularConstructor_Class_ShouldNotTriggerDiagnostic()
	{
		const string source = @"class C { public C(int x) { } }";
		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task Record_WithPositionalParameters_ShouldNotTriggerDiagnostic()
	{
		// Records' primary-like constructor (positional parameters) is allowed.
		const string source = @"public record R(int X);";
		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task RecordClassStyle_NoParameters_ShouldNotTriggerDiagnostic()
	{
		const string source = @"public record R { public R(int x) { } }";
		await VerifyNoDiagnosticsAsync(source);
	}
}