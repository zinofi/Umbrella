using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Umbrella.AspNetCore.WebUtilities.Health;

internal readonly record struct HealthReportModel(HealthStatus Status, IReadOnlyCollection<HealthReportResultModel> Results);