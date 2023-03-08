using System;
using System.Collections.Generic;
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
		private readonly Dictionary<string, string> _virtualStorage = new Dictionary<string, string>();
		private readonly bool _useVirtualStorage = DeviceInfo.DeviceType == DeviceType.Virtual && DeviceInfo.Platform == DevicePlatform.iOS;

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
				// The iOS Simulator doesn't support SecureStorage without enabling KeyChain which we don't want
				// to do so we are using an in-memory storage solution.
				if (_useVirtualStorage)
					return _virtualStorage.TryGetValue(key, out string value) ? value : null;

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
				if (_useVirtualStorage)
					_virtualStorage.Remove(key);
				else
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
				if (_useVirtualStorage)
				{
					_virtualStorage[key] = value;
				}
				else
				{
					// We need to ensure the item has been removed before setting a value
					// as iOS throws an exception when an item with the key already exists.
					_ = SecureStorage.Remove(key);
					await SecureStorage.SetAsync(key, value);
				}
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { key }, returnValue: true))
			{
				throw new UmbrellaXamarinException("There has been a problem setting the item with the specified key.", exc);
			}
		}
	}
}