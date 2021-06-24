using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Shared.Constants;
using Umbrella.Utilities;
using Umbrella.Utilities.Http;

namespace Umbrella.AppFramework.Http.Handlers
{
	/// <summary>
	/// A <see cref="DelegatingHandler"/> for use with the <see cref="IHttpClientFactory"/> infrastructure which
	/// refreshes the auth session if the response contains a new auth token.
	/// </summary>
	/// <seealso cref="System.Net.Http.DelegatingHandler" />
	public class RefreshedAuthTokenHandler : DelegatingHandler
	{
		private readonly IAppAuthHelper _authHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="RefreshedAuthTokenHandler"/> class.
		/// </summary>
		/// <param name="authHelper">The authentication helper.</param>
		public RefreshedAuthTokenHandler(
			IAppAuthHelper authHelper)
		{
			_authHelper = authHelper;
		}

		/// <inheritdoc />
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var response = await base.SendAsync(request, cancellationToken);

			if (response.Headers.TryGetValues(AppHttpHeaderName.NewAuthToken, out var values))
			{
				string? token = values.FirstOrDefault()?.Trim();

				if (!string.IsNullOrWhiteSpace(token))
					await _authHelper.GetCurrentClaimsPrincipalAsync(token);
			}
			else if (response.StatusCode is HttpStatusCode.Unauthorized && request.RequestUri.ToString().EndsWith("/auth/login", StringComparison.OrdinalIgnoreCase) is false)
			{
				string json = UmbrellaStatics.SerializeJson(new HttpProblemDetails { Title = "Logged Out", Detail = "You have been logged out due to inactivity. Please login again to continue." });
				response.Content = new StringContent(json, Encoding.UTF8, "application/problem+json");

				await _authHelper.LocalLogoutAsync();
			}

			return response;
		}
	}
}