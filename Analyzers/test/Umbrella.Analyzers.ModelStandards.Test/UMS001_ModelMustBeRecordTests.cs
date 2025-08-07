namespace Umbrella.Analyzers.ModelStandards.Test;

/// <summary>
/// Tests for UMS001: Model types must be records.
/// </summary>
public class UMS001_ModelMustBeRecordTests : AnalyzerTestBase
{
	[Fact]
	public async Task ModelClass_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public class UserModel
{
    public string Name { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ModelRecord_ShouldNotTriggerDiagnostic()
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
	public async Task ModelBaseClass_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public class UserModelBase
{
    public string Name { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, "UserModelBase"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", "UserModelBase"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", "UserModelBase")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ViewModelClass_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public class UserViewModel
{
    public string Name { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, "UserViewModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", "UserViewModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", "UserViewModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ViewModelBaseClass_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public class UserViewModelBase
{
    public string Name { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, "UserViewModelBase"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", "UserViewModelBase"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", "UserViewModelBase")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task NonModelClass_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public class UserService
{
    public string Name { get; set; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ModelClassWithOptOutAttribute_ShouldNotTriggerDiagnostic()
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

	[Theory]
	[InlineData("ProductModel")]
	[InlineData("OrderModel")]
	[InlineData("InventoryModel")]
	public async Task VariousModelNames_ShouldTriggerDiagnostic(string className)
	{
		string source = $@"
namespace TestProject;

public class {className}
{{
    public string Name {{ get; set; }}
}}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, className),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", className),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", className)
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Theory]
	[InlineData("ProductViewModel")]
	[InlineData("OrderViewModel")]
	[InlineData("InventoryViewModel")]
	public async Task VariousViewModelNames_ShouldTriggerDiagnostic(string className)
	{
		string source = $@"
namespace TestProject;

public class {className}
{{
    public string Name {{ get; set; }}
}}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, className),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", className),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", className)
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task NestedModelClass_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public class Container
{
    public class NestedModel
    {
        public string Name { get; set; }
    }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 6, 18, "NestedModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 8, 27, "Name", "NestedModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 27, "Name", "NestedModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task AbstractModelClass_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public abstract class UserModelBase
{
    public required string Name { get; init; }
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 23, "UserModelBase");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Theory]
	[InlineData("UserService")]
	[InlineData("ProductRepository")]
	[InlineData("OrderController")]
	[InlineData("MyModel")] // Doesn't end with exact patterns
	[InlineData("ModelUser")] // Pattern at beginning
	public async Task NonModelNamingPatterns_ShouldNotBeAnalyzed(string typeName)
	{
		string source = $@"
namespace TestProject;

public class {typeName}
{{
    public string Name {{ get; set; }}
}}";

		await VerifyNoDiagnosticsAsync(source);
	}
}