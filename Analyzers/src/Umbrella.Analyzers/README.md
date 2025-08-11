# Umbrella Analyzers

This package provides a set of Roslyn analyzers to enforce coding standards and best practices across .NET projects.

## Rules

### UA001 - Use pattern matching for null checks
- **Description**: Enforces the use of `is null` and `is not null` instead of `==` and `!=` for null checks.
- **Severity**: Warning

### UA002 - Use pattern matching for primitive and enum comparisons
- **Description**: Enforces the use of `is` and `is not` for comparing primitive types and enums instead of `==` and `!=`.
- **Severity**: Warning

### UA003 - Async methods should have a CancellationToken parameter
- **Description**: Ensures all async methods include a `CancellationToken cancellationToken = default` parameter.
- **Severity**: Warning

### UA004 - Async methods with CancellationToken should call ThrowIfCancellationRequested
- **Description**: Ensures all async methods that accept a `CancellationToken` call `cancellationToken.ThrowIfCancellationRequested()` as the first line of the method body.
- **Severity**: Warning

## Installation

Add the following to your `.csproj` file:

```xml
<PackageReference Include="Umbrella.Analyzers" Version="1.0.0" PrivateAssets="all" />
```

## Usage

1. Install the package in your .NET project.
2. The analyzers will automatically run during builds and in the IDE.
3. Fix any warnings reported by the analyzers to adhere to the enforced coding standards.

## Contributing

Contributions are welcome! Please submit issues or pull requests to improve the analyzers or add new rules.