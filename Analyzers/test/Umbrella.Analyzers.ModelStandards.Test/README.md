# Umbrella Model Standards Analyzer Tests

This project contains comprehensive unit tests for the `UmbrellaModelStandardsAnalyzer` using Microsoft's analyzer testing framework.

## Test Structure

### Test Categories

- **UMS001_ModelMustBeRecordTests**: Tests for the rule requiring model types to be records
- **UMS002_PropertiesMustBeRequiredTests**: Tests for the rule requiring properties to use the `required` keyword
- **UMS003_PropertiesMustBeGetterInitOnlyTests**: Tests for the rule requiring properties to be getter + init-only
- **UMS004_CollectionsMustBeReadOnlyTests**: Tests for the rule requiring collections to use `IReadOnlyCollection<T>`
- **IntegrationTests**: Tests that verify multiple rules working together
- **EdgeCaseTests**: Tests for boundary conditions and edge cases

### Base Test Class

All test classes inherit from `AnalyzerTestBase` which provides:
- `Diagnostic()` helper for creating expected diagnostic results
- `VerifyAnalyzerAsync()` for running analyzer tests with expected diagnostics
- `VerifyNoDiagnosticsAsync()` for verifying clean code produces no diagnostics

## Running Tests

### Command Line
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "ClassName=UMS001_ModelMustBeRecordTests"

# Run specific test method
dotnet test --filter "MethodName=ModelClass_ShouldTriggerDiagnostic"
```

### Visual Studio
- Use **Test Explorer** to view and run tests
- Right-click on test classes/methods to run specific tests
- View test results and coverage in the Test Explorer window

## Test Examples

### Basic Analyzer Test
```csharp
[Fact]
public async Task ModelClass_ShouldTriggerDiagnostic()
{
    const string source = @"
namespace TestProject;

public class UserModel
{
    public string Name { get; set; }
}";

    var expected = Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 4, 14, "UserModel");
    await VerifyAnalyzerAsync(source, expected);
}
```

### Multiple Diagnostics Test
```csharp
[Fact]
public async Task MultipleViolations_ShouldTriggerMultipleDiagnostics()
{
    const string source = @"
using System.Collections.Generic;

namespace TestProject;

public class UserModel
{
    public string Name { get; set; }
    public List<string> Tags { get; set; }
}";

    var expected = new[]
    {
        Diagnostic(UmbrellaModelStandardsAnalyzer.ModelMustBeRecordRule, 6, 14, "UserModel"),
        Diagnostic(UmbrellaModelStandardsAnalyzer.PropertiesMustBeRequiredRule, 8, 19, "Name", "UserModel"),
        // ... more diagnostics
    };
    await VerifyAnalyzerAsync(source, expected);
}
```

### No Diagnostics Test
```csharp
[Fact]
public async Task CompliantCode_ShouldNotTriggerDiagnostics()
{
    const string source = @"
namespace TestProject;

public record UserModel
{
    public required string Name { get; init; }
}";

    await VerifyNoDiagnosticsAsync(source);
}
```

## Adding New Tests

### When to Add Tests
- **New analyzer rules**: Create a new test class following the naming pattern `UMS00X_RuleNameTests`
- **Bug fixes**: Add regression tests to prevent the same issue
- **Edge cases**: Add to `EdgeCaseTests` or create specific test methods
- **New opt-out attributes**: Test both the attribute working and being ignored appropriately

### Test Naming Conventions
- Test class: `UMS00X_RuleNameTests`
- Test method: `Scenario_ShouldExpectedBehavior`
- Examples:
  - `ModelClass_ShouldTriggerDiagnostic`
  - `PropertyWithOptOut_ShouldNotTriggerDiagnostic`
  - `CompliantRecord_ShouldNotTriggerAnyDiagnostics`

### Best Practices
1. **Use meaningful test names** that describe the scenario and expected outcome
2. **Test both positive and negative cases** (should trigger vs shouldn't trigger)
3. **Include edge cases** like empty classes, static members, etc.
4. **Test opt-out attributes** to ensure they work correctly
5. **Use Theory tests** with InlineData for similar scenarios with different inputs
6. **Keep test code minimal** but complete enough to be realistic

## Coverage Goals

The test suite aims for:
- **100% line coverage** of analyzer logic
- **All diagnostic rules tested** in isolation and combination
- **All opt-out attributes tested** for proper functioning
- **Edge cases covered** to prevent unexpected behavior
- **Integration scenarios** to ensure rules work together correctly

## Dependencies

- **Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit**: Framework for testing analyzers
- **Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.XUnit**: Framework for testing code fixes
- **xUnit**: Test framework
- **coverlet**: Code coverage tool

## Key Test Scenarios Covered

### UMS001 (Model Must Be Record)
- ? Class vs record detection
- ? Various naming patterns (Model, ModelBase, ViewModel, ViewModelBase)
- ? Nested classes
- ? Abstract classes
- ? Opt-out attribute handling

### UMS002 (Properties Must Be Required)
- ? Missing `required` keyword
- ? Mixed required and optional properties
- ? Static properties (should be ignored)
- ? Opt-out attributes (UmbrellaAllowOptionalProperty, UmbrellaAllowLateInitialization)

### UMS003 (Properties Must Be Init-Only)
- ? get/set vs get/init patterns
- ? Expression-bodied properties
- ? Private setters and init accessors
- ? Properties with bodies vs auto-properties
- ? Opt-out attributes (UmbrellaAllowMutableProperty)

### UMS004 (Collections Must Be ReadOnly)
- ? List<T>, Array, ICollection<T> vs IReadOnlyCollection<T>
- ? String exclusion (not treated as collection)
- ? Custom collection types
- ? Various ReadOnly collection interfaces
- ? Opt-out attributes (UmbrellaAllowMutableCollection)

### Integration & Edge Cases
- ? Multiple rules triggering together
- ? Empty classes and records
- ? Generic types and constraints
- ? Interface implementations
- ? Inheritance scenarios
- ? Partial classes
- ? Methods, fields, events, indexers (should be ignored)
- ? Record parameters vs properties

## Debugging Tests

### Common Issues
1. **Line/column numbers off**: Check for extra whitespace or line breaks in test source
2. **Diagnostic not triggered**: Verify the test code actually violates the rule
3. **Multiple diagnostics**: Ensure all expected diagnostics are listed
4. **Wrong diagnostic arguments**: Check the message format parameters

### Debugging Tips
- Use the debugger to step through analyzer logic
- Add temporary `Console.WriteLine()` statements in the analyzer
- Verify test source code compiles correctly
- Check that using statements are included when needed

## Running Code Coverage

```bash
# Generate code coverage report
dotnet test --collect:"XPlat Code Coverage"

# For detailed HTML reports (install reportgenerator first)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

This comprehensive test suite ensures your analyzer works correctly across all scenarios and provides confidence for future changes and maintenance.