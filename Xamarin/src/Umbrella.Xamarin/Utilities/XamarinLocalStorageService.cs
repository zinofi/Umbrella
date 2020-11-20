using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Xamarin.Exceptions;
using Xamarin.Essentials;

namespace Umbrella.Xamarin.Utilities
{
	/// <summary>
	/// A persistent storage service used to store string values used by the app using the <see cref="SecureStorage"/> mechanism.
	/// </summary>
	/// <seealso cref="IAppLocalStorageService" />
	public class XamarinLocalStorageService : IAppLocalStorageService
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="XamarinLocalStorageService"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public XamarinLocalStorageService(ILogger<XamarinLocalStorageService> logger)
		{
			_logger = logger;
		}

		/// <inheritdoc />
		public async ValueTask<string?> GetAsync(string key)
		{
			try
			{
				return await SecureStorage.GetAsync(key);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { key }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem retrieving the item with the specified key.", exc);
			}
		}

		/// <inheritdoc />
		public ValueTask RemoveAsync(string key)
		{
			try
			{
				SecureStorage.Remove(key);

				return default;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { key }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem removing the item with the specified key.", exc);
			}
		}

		/// <inheritdoc />
		public async ValueTask SetAsync(string key, string value)
		{
			try
			{
				await SecureStorage.SetAsync(key, value);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { key }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem setting the item with the specified key.", exc);
			}
		}
	}
}