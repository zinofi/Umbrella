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

### ? **Automatic Code Fixes Available!**

When targeting **.NET 8.0 or .NET 9.0**, this analyzer provides automatic code fixes for all violations:

- **UMS001**: Convert class to record
- **UMS002**: Add `required` modifier to properties
- **UMS003**: Fix property accessors (change `set` to `init`, add missing `get`)
- **UMS004**: Change collection types to `IReadOnlyCollection<T>`

### Target Framework Support

| Target Framework | Analyzer (Diagnostics) | Code Fixes |
|-----------------|------------------------|------------|
| .NET Standard 2.0 | ? Full Support | ? Not Available |
| .NET 8.0 | ? Full Support | ? **Full Support** |
| .NET 9.0 | ? Full Support | ? **Full Support** |

### How to Use Code Fixes

1. **In Visual Studio**: Click the lightbulb ?? icon that appears when hovering over violations
2. **Quick Actions**: Use `Ctrl+.` (Windows) or `Cmd+.` (Mac) when cursor is on a violation
3. **Fix All**: Use "Fix All Occurrences" to fix multiple violations at once

### Example Code Fix in Action

**Before (with violation):**
```csharp
public class UserModel  // UMS001 violation
{
    public string Name { get; set; }  // UMS002, UMS003 violations
    public List<string> Tags { get; init; }  // UMS002, UMS004 violations
}
```

**After applying code fixes:**
```csharp
public record UserModel
{
    public required string Name { get; init; }
    public required IReadOnlyCollection<string> Tags { get; init; }
}
```

## Usage

1. Install the `Umbrella.Analyzers.ModelStandards` NuGet package
2. The analyzer will automatically run during builds and show **compilation errors** for violations
3. **For .NET 8.0+ projects**: Use the automatic code fixes via IDE quick actions
4. **For .NET Standard 2.0 projects**: Manually apply fixes based on diagnostic messages
5. Use opt-out attributes with proper justifications when standards cannot be followed

## Requirements

### For Analyzer (Diagnostics):
- .NET Standard 2.0 or higher
- Visual Studio 2019+ or equivalent tooling with Roslyn analyzer support

### For Code Fixes:
- **.NET 8.0 or .NET 9.0** target framework
- Visual Studio 2022+ with C# 11.0+ support
- For projects using the analyzer: .NET framework supporting `required` keyword and `init` accessors

## Installation

```xml
<PackageReference Include="Umbrella.Analyzers.ModelStandards" Version="1.0.0" PrivateAssets="all" />
```

**Note**: The analyzer automatically adapts based on your project's target framework - providing full code fix functionality for .NET 8+ and diagnostics-only support for .NET Standard 2.0.

## Diagnostic Severity

All analyzer rules are configured with **Error** severity, meaning violations will prevent compilation and must be fixed before the build can succeed. This ensures strict adherence to Umbrella model standards.