using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Umbrella.AspNetCore.WebUtilities.Routing
{
	/// <summary>
	/// A route transformer which slugifies route parameters and returns them in lowercase, e.g. transforms "ManageAccount" to "manage-account"
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Routing.IOutboundParameterTransformer" />
	public class SlugifyParameterTransformer : IOutboundParameterTransformer
	{
		private static readonly Regex _urlTransformer = new("([a-z])([A-Z])", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <inheritdoc />
		public string? TransformOutbound(object? value) => value is null ? null : _urlTransformer.Replace(value.ToString() ?? string.Empty, "$1-$2").ToLowerInvariant();
	}
}