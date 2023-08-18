using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Umbrella.AspNetCore.WebUtilities.Health;

internal record HealthReportResultModel(string Name, HealthStatus Status, string? Description, Dictionary<string, string?> Data);
