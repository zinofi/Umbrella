namespace Umbrella.Analyzers.ModelStandards.Test;

public class DebugTests : AnalyzerTestBase
{
	[Fact]
	public async Task Debug_SimpleUMS001Test()
	{
		const string source = @"
namespace TestProject;

public class UserModel
{
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, "UserModel");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task DebugMixedRequiredAndOptionalProperties_ShowAllDiagnostics()
	{
		const string source = @"
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    public string Email { get; init; }
    
    [UmbrellaAllowOptionalProperty(""Framework sets this value"")]
    public string? Id { get; init; }
    
    public required int Age { get; init; }
    public string? Phone { get; init; }
}";

		// Just run without any expected diagnostics to see what the analyzer produces
		await VerifyNoDiagnosticsAsync(source);
	}
}
