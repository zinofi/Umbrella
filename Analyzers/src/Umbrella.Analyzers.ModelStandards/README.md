# Umbrella Model Standards Analyzer

This Roslyn analyzer enforces coding standards for model classes and view models in the Umbrella framework.

## Rules

### UMS001 - Model types must be records
Model types (classes ending with "Model", "ModelBase", "ViewModel", or "ViewModelBase") should be defined as records for better immutability guarantees.

**? Bad:**
```csharp
public class UserModel
{
    public string Name { get; set; }
}
```

**? Good:**
```csharp
public record UserModel
{
    public required string Name { get; init; }
}
```

### UMS002 - Model properties must use the required keyword
Properties in model types should use the `required` keyword to ensure they are initialized.

**? Bad:**
```csharp
public record UserModel
{
    public string Name { get; init; }
}
```

**? Good:**
```csharp
public record UserModel
{
    public required string Name { get; init; }
}
```

### UMS003 - Model properties must have getter and be init-only
Properties should have a getter and be init-only instead of mutable setters.

**? Bad:**
```csharp
public record UserModel
{
    public required string Name { get; set; }
}
```

**? Good:**
```csharp
public record UserModel
{
    public required string Name { get; init; }
}
```

### UMS004 - Collection properties must use IReadOnlyCollection<T>
Collection properties should use `IReadOnlyCollection<T>` for better immutability.

**? Bad:**
```csharp
public record UserModel
{
    public required List<string> Tags { get; init; }
}
```

**? Good:**
```csharp
public record UserModel
{
    public required IReadOnlyCollection<string> Tags { get; init; }
}
```

## Opt-out Attributes

You can opt out of specific rules using the following attributes. **All opt-out attributes now require a justification parameter** to ensure proper documentation of why standards are being bypassed.

### UmbrellaExcludeFromModelStandardsAttribute
Excludes an entire type from model standards enforcement.

```csharp
[UmbrellaExcludeFromModelStandards("Legacy code that cannot be easily converted")]
public class LegacyUserModel
{
    public string Name { get; set; }
}
```

### UmbrellaAllowOptionalPropertyAttribute
Allows a property to skip the 'required' keyword requirement.

```csharp
public record UserModel
{
    public required string Name { get; init; }
    
    [UmbrellaAllowOptionalProperty("This property is set by the framework after creation")]
    public string? Id { get; init; }
}
```

### UmbrellaAllowMutablePropertyAttribute
Allows a property to be mutable (have a setter).

```csharp
public record UserModel
{
    public required string Name { get; init; }
    
    [UmbrellaAllowMutableProperty("Needs to be updated during user session")]
    public required string LastActivity { get; set; }
}
```

### UmbrellaAllowLateInitializationAttribute
Indicates a property will be initialized after object creation.

```csharp
public record UserModel
{
    public required string Name { get; init; }
    
    [UmbrellaAllowLateInitialization("Set by dependency injection")]
    public IUserService UserService { get; set; }
}
```

### UmbrellaAllowMutableCollectionAttribute
Allows a collection property to use mutable collection types.

```csharp
public record UserModel
{
    public required string Name { get; init; }
    
    [UmbrellaAllowMutableCollection("Needs to be modified during processing")]
    public required List<string> Tags { get; init; }
}
```

## Code Fixes

**Note:** Automatic code fixes are currently not available in this version of the analyzer. The analyzer provides diagnostics and warnings, but you'll need to manually apply the fixes.

### Future Code Fix Support

To enable automatic code fixes in a future version, the following would be required:

1. **Target Framework**: The analyzer would need to target **.NET 6.0 or higher** instead of .NET Standard 2.0
2. **Package Dependencies**: Add `Microsoft.CodeAnalysis.Workspaces.Common` package reference
3. **Implementation**: Implement a full `CodeFixProvider` with `ExportCodeFixProvider` attribute

The current version targets .NET Standard 2.0 for maximum compatibility across different development environments and CI/CD scenarios.

## Usage

1. Install the `Umbrella.Analyzers.ModelStandards` NuGet package
2. The analyzer will automatically run during builds and show warnings for violations
3. Use the opt-out attributes with proper justifications when standards cannot be followed
4. Manually apply the suggested fixes based on the diagnostic messages

## Requirements

- .NET Standard 2.0 or higher
- Visual Studio 2019+ or equivalent tooling with Roslyn analyzer support
- For projects using the analyzer: .NET framework supporting `required` keyword and `init` accessors (C# 9.0+/C# 11.0+)