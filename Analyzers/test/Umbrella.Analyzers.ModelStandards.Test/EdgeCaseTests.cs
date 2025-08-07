namespace Umbrella.Analyzers.ModelStandards.Test;

/// <summary>
/// Tests to verify edge cases and boundary conditions for the analyzer.
/// </summary>
public class EdgeCaseTests : AnalyzerTestBase
{
	[Fact]
	public async Task EmptyModelClass_ShouldOnlyTriggerRecordRule()
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
	public async Task EmptyModelRecord_ShouldNotTriggerAnyDiagnostics()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task StaticPropertiesInModel_ShouldNotTriggerPropertyRules()
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

	[Fact]
	public async Task ConstFieldsInModel_ShouldNotTriggerAnyRules()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public const string DefaultName = ""Unknown"";
    public required string Name { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task MethodsInModel_ShouldNotTriggerAnyRules()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    
    public string GetDisplayName()
    {
        return Name.ToUpper();
    }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ModelWithGenericConstraints_ShouldWorkCorrectly()
	{
		const string source = @"
namespace TestProject;

public record GenericModel<T> where T : class
{
    public required T Value { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ModelImplementingInterface_ShouldWorkCorrectly()
	{
		const string source = @"
namespace TestProject;

public interface IModel
{
    string Name { get; }
}

public record UserModel : IModel
{
    public required string Name { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ModelWithInheritance_ShouldAnalyzeBothLevels()
	{
		const string source = @"
namespace TestProject;

public record BaseModel
{
    public required string Id { get; init; }
}

public record UserModel : BaseModel
{
    public required string Name { get; init; }
    public string Email { get; set; }
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 12, 19, "Email", "UserModel");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PartialModelClasses_ShouldAnalyzeBothParts()
	{
		const string source = @"
namespace TestProject;

public partial class UserModel
{
    public string Name { get; set; }
}

public partial class UserModel
{
    public string Email { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 22, "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 9, 22, "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 6, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 19, "Name", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 11, 19, "Email", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 11, 19, "Email", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ModelWithIndexer_ShouldNotTriggerPropertyRules()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    private readonly Dictionary<string, object> _data = new();
    
    public required string Name { get; init; }
    
    public object this[string key]
    {
        get => _data[key];
        set => _data[key] = value;
    }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ModelWithEvents_ShouldNotTriggerAnyRules()
	{
		const string source = @"
using System;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    
    public event EventHandler? NameChanged;
    
    protected virtual void OnNameChanged()
    {
        NameChanged?.Invoke(this, EventArgs.Empty);
    }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Theory]
	[InlineData("UserModel")]
	[InlineData("ProductModel")]
	[InlineData("OrderViewModel")]
	[InlineData("BaseModelBase")]
	[InlineData("TestViewModelBase")]
	public async Task VariousNamingPatterns_ShouldBeRecognizedAsModels(string typeName)
	{
		string source = $@"
namespace TestProject;

public class {typeName}
{{
    public string Name {{ get; set; }}
}}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, typeName);
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

	[Fact]
	public async Task ModelWithInitOnlyCollectionProperty_ShouldTriggerOnlyCollectionRule()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required List<string> Tags { get; init; }
}";

		var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 30, "Tags", "UserModel");
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ModelWithRecordParameters_ShouldNotTriggerDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel(string Name, IReadOnlyCollection<string> Tags);";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task ModelWithMixedParametersAndProperties_ShouldAnalyzePropertiesOnly()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel(string Name)
{
    public string Email { get; set; }
    public required List<string> Tags { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 8, 19, "Email", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 19, "Email", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 9, 30, "Tags", "UserModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task NestedGenericModel_ShouldWorkCorrectly()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record ResponseModel<T>
{
    public required T Data { get; init; }
    public required IReadOnlyCollection<string> Messages { get; init; }
    public string Status { get; set; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 10, 19, "Status", "ResponseModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 10, 19, "Status", "ResponseModel")
		};
		await VerifyAnalyzerAsync(source, expected);
	}
}