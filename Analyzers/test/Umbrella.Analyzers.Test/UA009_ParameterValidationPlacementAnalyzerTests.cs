namespace Umbrella.Analyzers.Test;

public class UA009_ParameterValidationPlacementAnalyzerTests : AnalyzerTestBase<ParameterValidationPlacementAnalyzer>
{
	[Fact]
	public async Task GuardCall_AfterTry_ShouldTriggerDiagnostic()
	{
		const string source = @"using CommunityToolkit.Diagnostics; class C { void M(string x) { try { } catch { } Guard.IsNotNull(x); } }";
		var expected = Diagnostic(ParameterValidationPlacementAnalyzer.Rule, 1, 103, "M");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task GuardCall_BeforeTry_ShouldNotTriggerDiagnostic()
	{
		const string source = @"using CommunityToolkit.Diagnostics; class C { void M(string x) { Guard.IsNotNull(x); try { } catch { } } }";
		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ThrowHelper_AfterTry_ShouldTriggerDiagnostic()
	{
		const string source = @"class C { void M(string x) { try { } catch { } ArgumentNullException.ThrowIfNull(x); } }";
		var expected = Diagnostic(ParameterValidationPlacementAnalyzer.Rule, 1, 70, "M");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ThrowHelper_BeforeTry_ShouldNotTriggerDiagnostic()
	{
		const string source = @"class C { void M(string x) { ArgumentNullException.ThrowIfNull(x); try { } catch { } } }";
		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task DirectThrow_AfterTry_ShouldTriggerDiagnostic()
	{
		const string source = @"class C { void M(string x) { try { } catch { } throw new ArgumentNullException(nameof(x)); } }";
		var expected = Diagnostic(ParameterValidationPlacementAnalyzer.Rule, 1, 66, "M");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task DirectThrow_BeforeTry_ShouldNotTriggerDiagnostic()
	{
		const string source = @"class C { void M(string x) { if (x == null) throw new ArgumentNullException(nameof(x)); try { } catch { } } }";
		await VerifyNoDiagnosticsAsync(source);
	}
}
