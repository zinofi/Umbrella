using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Umbrella.AspNetCore.WebUtilities.Health;

internal readonly record struct HealthReportResultModel(string Name, HealthStatus Status, string? Description, Dictionary<string, string?> Data);
