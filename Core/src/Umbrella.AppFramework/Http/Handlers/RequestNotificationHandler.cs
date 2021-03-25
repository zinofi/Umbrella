using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.AppFramework.Http.Handlers.Options;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AppFramework.Http.Handlers
{
	/// <summary>
	/// A <see cref="DelegatingHandler"/> for use with the <see cref="IHttpClientFactory"/> infrastructure which notifies users
	/// that there is a request in progress by displaying and hiding a loading indicator.
	/// </summary>
	/// <seealso cref="System.Net.Http.DelegatingHandler" />
	public class RequestNotificationHandler : DelegatingHandler
	{
		private readonly RequestNotificationHandlerOptions _options;
		private readonly ILoadingScreenUtility _loadingScreenUtility;

		/// <summary>
		/// Initializes a new instance of the <see cref="RequestNotificationHandler"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="loadingScreenUtility">The loading screen utility.</param>
		public RequestNotificationHandler(
			RequestNotificationHandlerOptions options,
			ILoadingScreenUtility loadingScreenUtility)
		{
			_options = options;
			_loadingScreenUtility = loadingScreenUtility;
		}

		/// <inheritdoc />
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (_options.Exclusions.Any(x => request.RequestUri.AbsolutePath.Equals(x.Path, StringComparison.InvariantCultureIgnoreCase) && request.Method == x.Method))
				return await base.SendAsync(request, cancellationToken);

			try
			{
				_loadingScreenUtility.Show();

				HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

				return response;
			}
			finally
			{
				_loadingScreenUtility.Hide();
			}
		}
	}
}