namespace Umbrella.WebUtilities.Versioning.Models;

/// <summary>
/// Represents version information about the current system.
/// </summary>
/// <param name="Version">The system version.</param>
/// <param name="DatabaseVersion">The optional version of the system database.</param>
/// <seealso cref="IEquatable{SystemVersionModel}" />
public record SystemVersionModel(string Version, string? DatabaseVersion);