namespace Umbrella.Analyzers.ModelStandards.Test;

/// <summary>
/// Tests for UMS003: Model properties must have getter and be init-only.
/// </summary>
public class UMS003_PropertiesMustBeGetterInitOnlyTests : AnalyzerTestBase
{
	[Fact]
	public async Task PropertyWithGetSet_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; set; }
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 28, "Name", "UserModel");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithGetInit_ShouldNotTriggerDiagnostic()
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
	public async Task PropertyWithOnlySet_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { set; }
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 28, "Name", "UserModel");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithOnlyGet_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; }
    
    public UserModel(string name)
    {
        Name = name;
    }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task AutoPropertyWithGetSet_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; set; }
    public required int Age { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 28, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 7, 25, "Age", "UserModel")
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
    
    [UmbrellaAllowMutableProperty(""Needs to be updated during user session"")]
    public required string LastActivity { get; set; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ExpressionBodiedProperty_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    
    public string FullName => $""{FirstName} {LastName}"";
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyWithPrivateSetInit_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; private init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyWithPrivateSetSet_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; private set; }
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 28, "Name", "UserModel");
		await VerifyAnalyzerAsync(source, expected);
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
    public string Name { get; set; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task MixedAccessorProperties_ShouldTriggerSelectiveDiagnostics()
	{
		const string source = @"
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    public required string Email { get; set; }
    
    [UmbrellaAllowMutableProperty(""Framework needs to update this"")]
    public required string Status { get; set; }
    
    public required int Age { get; }
    public required string Phone { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 28, "Email", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 14, 28, "Phone", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithBodyImplementation_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    private string _name = string.Empty;
    
    public required string Name 
    { 
        get => _name; 
        set => _name = value; 
    }
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 28, "Name", "UserModel");
		await VerifyAnalyzerAsync(source, expected);
	}
}