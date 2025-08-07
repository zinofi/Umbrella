namespace Umbrella.Analyzers.ModelStandards.Test;

/// <summary>
/// Integration tests that verify multiple analyzer rules working together.
/// </summary>
public class IntegrationTests : AnalyzerTestBase
{
	[Fact]
	public async Task CompletelyNonCompliantClass_ShouldTriggerAllRelevantDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public class UserModel
{
    public string Name { get; set; }
    public List<string> Tags { get; set; }
    public int Age { get; set; }
}";

		var expected = new[]
		{
			// UMS001: Must be record
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 6, 14, "UserModel"),
			
			// UMS002: Properties must be required
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 8, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 9, 21, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 10, 16, "Age", "UserModel"),
			
			// UMS003: Properties must be init-only
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 9, 21, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 10, 16, "Age", "UserModel"),
			
			// UMS004: Collections must be read-only
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 9, 21, "Tags", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task CompletelyCompliantRecord_ShouldNotTriggerAnyDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    public required IReadOnlyCollection<string> Tags { get; init; }
    public required int Age { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task MixedComplianceRecord_ShouldTriggerSelectiveDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    public string Email { get; set; }
    public required List<string> Tags { get; init; }
    public required IReadOnlyCollection<string> Categories { get; init; }
}";

		var expected = new[]
		{
			// UMS002: Email missing required
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 9, 19, "Email", "UserModel"),
			
			// UMS003: Email has setter instead of init
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 9, 19, "Email", "UserModel"),
			
			// UMS004: Tags should be IReadOnlyCollection
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 10, 30, "Tags", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ClassWithGlobalOptOut_ShouldNotTriggerAnyDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

[UmbrellaExcludeFromModelStandards(""This is legacy code that we cannot refactor right now"")]
public class UserModel
{
    public string Name { get; set; }
    public List<string> Tags { get; set; }
    public int Age { get; set; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task RecordWithSelectiveOptOuts_ShouldTriggerAppropriateRemainingDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    
    [UmbrellaAllowOptionalProperty(""Framework will set this"")]
    public string? Id { get; init; }
    
    [UmbrellaAllowMutableProperty(""Needs to be updated during runtime"")]
    public required string Status { get; set; }
    
    [UmbrellaAllowMutableCollection(""Processing requires mutation"")]
    public required List<string> ProcessingQueue { get; init; }
    
    public string Email { get; set; }
    public required List<string> Tags { get; init; }
}";

		var expected = new[]
		{
			// UMS002: Email missing required
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 19, 19, "Email", "UserModel"),
			
			// UMS003: Email has setter instead of init
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 19, 19, "Email", "UserModel"),
			
			// UMS004: Tags should be IReadOnlyCollection (ProcessingQueue is opted out)
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 20, 30, "Tags", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task MultipleModelTypes_ShouldAnalyzeAllRelevantTypes()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public class UserModel
{
    public string Name { get; set; }
}

public record ProductModel
{
    public required string Name { get; init; }
    public List<string> Categories { get; init; }
}

public class OrderViewModel
{
    public required string OrderId { get; set; }
}

public class NonModelClass
{
    public string Name { get; set; }
}";

		var expected = new[]
		{
			// UserModel diagnostics
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 6, 14, "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 8, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 19, "Name", "UserModel"),
			
			// ProductModel diagnostics
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 14, 21, "Categories", "ProductModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 14, 21, "Categories", "ProductModel"),
			
			// OrderViewModel diagnostics
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 17, 14, "OrderViewModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 19, 28, "OrderId", "OrderViewModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task NestedModelsAndComplexScenarios_ShouldHandleCorrectly()
	{
		const string source = @"
using System.Collections.Generic;
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    public required AddressModel Address { get; init; }
    public required IReadOnlyCollection<OrderModel> Orders { get; init; }
}

public record AddressModel
{
    public required string Street { get; init; }
    public string? City { get; init; }
}

[UmbrellaExcludeFromModelStandards(""Legacy integration model"")]
public class OrderModel
{
    public string OrderId { get; set; }
    public List<string> Items { get; set; }
}";

		var expected = new[]
		{
			// AddressModel - City missing required
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 17, 20, "City", "AddressModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task EmptyModelTypes_ShouldTriggerAppropriateRules()
	{
		const string source = @"
namespace TestProject;

public class EmptyUserModel
{
}

public record EmptyProductModel
{
}

public class EmptyNonModel
{
}";

		var expected = new[]
		{
			// Only UMS001 should trigger for empty model classes
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, "EmptyUserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ModelWithMethodsAndFields_ShouldOnlyAnalyzeProperties()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    private readonly string _defaultName = ""Unknown"";
    public const string CompanyName = ""MyCompany"";
    
    public required string Name { get; init; }
    public string Email { get; set; }
    public required List<string> Tags { get; init; }
    
    public string GetDisplayName()
    {
        return Name.ToUpper();
    }
    
    public static string GetDefaultName() => ""Default"";
}";

		var expected = new[]
		{
			// UMS002: Email missing required
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 12, 19, "Email", "UserModel"),
			
			// UMS003: Email has setter instead of init
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 12, 19, "Email", "UserModel"),
			
			// UMS004: Tags should be IReadOnlyCollection
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 13, 30, "Tags", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}
}