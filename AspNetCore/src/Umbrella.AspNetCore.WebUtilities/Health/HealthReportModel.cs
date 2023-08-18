using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Umbrella.AspNetCore.WebUtilities.Health;

internal record HealthReportModel(HealthStatus Status, IReadOnlyCollection<HealthReportResultModel> Results);