using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Umbrella.AspNetCore.WebUtilities.Health;

/// <summary>
/// Contains methods used to write a <see cref="HealthReport" /> to a <see cref="HttpResponse"/>.
/// </summary>
public static class HealthReportWriter
{
	private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
	{
		WriteIndented = true
	};

	/// <summary>
	/// Writes the specified report to the <see cref="HttpContext.Response"/> as JSON.
	/// </summary>
	/// <param name="context">The Http Context.</param>
	/// <param name="healthReport">The report.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public static Task WriteResponseAsJsonAsync(HttpContext context, HealthReport healthReport)
	{
		context.Response.ContentType = "application/json; charset=utf-8";

		var report = new HealthReportModel(
			healthReport.Status,
			healthReport.Entries.Select(x => new HealthReportResultModel(x.Key, x.Value.Status, x.Value.Description, x.Value.Data.ToDictionary(x => x.Key, x => x.Value.ToString()))).ToArray());

		return context.Response.WriteAsJsonAsync(
			report,
			_jsonOptions,
			context.RequestAborted);
	}
}