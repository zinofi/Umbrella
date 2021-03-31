using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Xamarin.Exceptions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Utilities
{
	/// <summary>
	/// A utility used to perform navigation to a specific URI.
	/// </summary>
	public class UriNavigator : IUriNavigator
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="UriNavigator"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public UriNavigator(ILogger<UriNavigator> logger)
		{
			_logger = logger;
		}

		/// <inheritdoc />
		public async ValueTask OpenAsync(string uri, bool openInNewWindow)
		{
			try
			{
				await Device.InvokeOnMainThreadAsync(async () => await Browser.OpenAsync(uri, openInNewWindow ? BrowserLaunchMode.External : BrowserLaunchMode.SystemPreferred));
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { uri, openInNewWindow }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem opening the specified URI.", exc);
			}
		}
	}
}