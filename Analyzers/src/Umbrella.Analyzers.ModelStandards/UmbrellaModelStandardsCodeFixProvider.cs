namespace Umbrella.Analyzers.ModelStandards;

/// <summary>
/// Code fix provider for Umbrella model standards analyzer.
/// </summary>
/// <remarks>
/// <para>
/// Code fixes are currently disabled as they require targeting .NET 6.0 or higher,
/// but this analyzer package targets .NET Standard 2.0 for maximum compatibility.
/// </para>
/// <para>
/// To enable code fixes in a future version, the project would need to:
/// 1. Target .NET 6.0+ instead of .NET Standard 2.0
/// 2. Add Microsoft.CodeAnalysis.Workspaces.Common package reference
/// 3. Implement the CodeFixProvider class with ExportCodeFixProvider attribute
/// </para>
/// <para>
/// The analyzer still functions fully and provides diagnostics - only the automatic
/// code fixing functionality is unavailable.
/// </para>
/// </remarks>
internal class UmbrellaModelStandardsCodeFixProvider
{
	// Code fixes require Microsoft.CodeAnalysis.Workspaces which is not available for .NET Standard 2.0
	// See class documentation for requirements to enable code fixes
}