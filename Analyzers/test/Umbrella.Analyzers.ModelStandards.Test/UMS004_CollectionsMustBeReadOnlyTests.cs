namespace Umbrella.Analyzers.ModelStandards.Test;

/// <summary>
/// Tests for UMS004: Collection properties must use IReadOnlyCollection&lt;T&gt;.
/// </summary>
public class UMS004_CollectionsMustBeReadOnlyTests : AnalyzerTestBase
{
	[Fact]
	public async Task PropertyWithList_ShouldTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required List<string> Tags { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 30, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 30, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 30, "Tags", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithIReadOnlyCollection_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required IReadOnlyCollection<string> Tags { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyWithICollection_ShouldTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required ICollection<string> Tags { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 37, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 37, "Tags", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithArray_ShouldTriggerDiagnostic()
	{
		const string source = @"
namespace TestProject;

public record UserModel
{
    public required string[] Tags { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 6, 26, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 6, 26, "Tags", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithIEnumerable_ShouldTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required IEnumerable<string> Tags { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 37, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 37, "Tags", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyWithOptOutAttribute_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required IReadOnlyCollection<string> Tags { get; init; }
    
    [UmbrellaAllowMutableCollection(""Needs to be modified during processing"")]
    public required List<string> Categories { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyWithString_ShouldNotTriggerDiagnostic()
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
	public async Task MultipleCollectionProperties_ShouldTriggerMultipleDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required List<string> Tags { get; init; }
    public required string[] Categories { get; init; }
    public required ICollection<int> Numbers { get; init; }
    public required IReadOnlyCollection<string> ReadOnlyTags { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 30, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 30, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 30, "Tags", "UserModel"),

			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 9, 26, "Categories", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 9, 26, "Categories", "UserModel"),

			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 10, 34, "Numbers", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 10, 34, "Numbers", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task PropertyInClassWithOptOutAttribute_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

[UmbrellaExcludeFromModelStandards(""Legacy code that cannot be easily converted"")]
public class UserModel
{
    public List<string> Tags { get; set; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task PropertyInNonModelClass_ShouldNotTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserService
{
    public List<string> Tags { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task GenericCollectionTypes_ShouldTriggerDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required HashSet<string> UniqueNames { get; init; }
    public required Dictionary<string, int> Scores { get; init; }
    public required Queue<string> ProcessingQueue { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 33, "UniqueNames", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 33, "UniqueNames", "UserModel"),

			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 9, 39, "Scores", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 9, 39, "Scores", "UserModel"),

			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 10, 31, "ProcessingQueue", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 10, 31, "ProcessingQueue", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task ReadOnlyCollectionVariants_ShouldNotTriggerDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required IReadOnlyCollection<string> Tags { get; init; }
    public required IReadOnlyList<string> OrderedTags { get; init; }
    public required IReadOnlyDictionary<string, int> Scores { get; init; }
}";

		await VerifyNoDiagnosticsAsync(source);
	}

	[Fact]
	public async Task MixedCollectionAndNonCollectionProperties_ShouldTriggerSelectiveDiagnostics()
	{
		const string source = @"
using System.Collections.Generic;
using Umbrella.Analyzers.ModelStandards;

namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
    public required List<string> Tags { get; init; }
    public required int Age { get; init; }
    
    [UmbrellaAllowMutableCollection(""Special processing needed"")]
    public required ICollection<string> ProcessingList { get; init; }
    
    public required IReadOnlyCollection<string> ReadOnlyTags { get; init; }
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 10, 30, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 10, 30, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 10, 30, "Tags", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}

	[Fact]
	public async Task CollectionWithImplementedInterface_ShouldTriggerDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;

namespace TestProject;

public record UserModel
{
    public required CustomCollection<string> Tags { get; init; }
}

public class CustomCollection<T> : List<T>
{
}";

		var expected = new[]
		{
			Diagnostic(UmbrellaModelStandardsAnalyzer.CollectionsMustBeReadOnlyRule, 8, 39, "Tags", "UserModel"),
			Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeGetterInitOnlyRule, 8, 39, "Tags", "UserModel"),
		};
		await VerifyAnalyzerAsync(source, expected);
	}
}