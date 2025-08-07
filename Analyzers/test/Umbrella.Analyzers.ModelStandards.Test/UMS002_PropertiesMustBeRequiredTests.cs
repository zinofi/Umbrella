namespace Umbrella.Analyzers.ModelStandards.Test;

/// <summary>
/// Tests for UMS002: Model properties must use the required keyword.
/// </summary>
public class UMS002_PropertiesMustBeRequiredTests : AnalyzerTestBase
{
	[Fact]
	public async Task PropertyWithoutRequired_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public string Name { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithRequired_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task MultiplePropertiesWithoutRequired_ShouldTriggerMultipleDiagnostics()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public string Name { get; init; }
    public int Age { get; init; }
    public string Email { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 7, 16, "Age", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 7, 16, "Age", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 8, 19, "Email", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 19, "Email", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithOptOutAttribute_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    
    [UmbrellaAllowOptionalProperty(""This property is set by the framework after creation"")]
    public string? Id { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyWithLateInitializationAttribute_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    
    [UmbrellaAllowLateInitialization(""Set by dependency injection"")]
    public IUserService? UserService { get; set; }
}

public interface IUserService { }";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyInClassWithOptOutAttribute_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

[UmbrellaExcludeFromModelStandards(""Legacy code that cannot be easily converted"")]
public class UserModel
{
    public string Name { get; set; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyInNonModelClass_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserService
{
    public string Name { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task MixedRequiredAndOptionalProperties_ShouldTriggerSelectiveDiagnostics()
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

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 9, 19, "Email", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 9, 19, "Email", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 15, 20, "Phone", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 15, 20, "Phone", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithComplexType_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public Address HomeAddress { get; init; }
}

public record Address
{
    public required string Street { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 20, "HomeAddress", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 20, "HomeAddress", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task StaticProperty_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public static string DefaultName { get; set; }
    public required string Name { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}
}