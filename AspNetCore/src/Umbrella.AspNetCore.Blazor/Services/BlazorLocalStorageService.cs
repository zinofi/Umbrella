﻿using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Services.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services;

/// <summary>
/// A persistent local storage service used to store string values used by the app using the <see cref="ILocalStorageService"/>.
/// </summary>
/// <seealso cref="IAppLocalStorageService" />
public class BlazorLocalStorageService : IAppLocalStorageService
{
	private readonly ILogger _logger;
	private readonly ILocalStorageService _storageService;

	/// <summary>
	/// Initializes a new instance of the <see cref="BlazorLocalStorageService"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="storageService">The storage service.</param>
	public BlazorLocalStorageService(
		ILogger<BlazorLocalStorageService> logger,
		ILocalStorageService storageService)
	{
		_logger = logger;
		_storageService = storageService;
	}

	/// <inheritdoc />
	public async ValueTask<string?> GetAsync(string key)
	{
		try
		{
			return await _storageService.GetItemAsStringAsync(key);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { key }))
		{
			throw new UmbrellaBlazorException("There has been a problem retrieving the item with the specified key.", exc);
		}
	}

	/// <inheritdoc />
	public async ValueTask RemoveAsync(string key)
	{
		try
		{
			await _storageService.RemoveItemAsync(key);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { key }))
		{
			throw new UmbrellaBlazorException("There has been a problem removing the item with the specified key.", exc);
		}
	}

	/// <inheritdoc />
	public async ValueTask SetAsync(string key, string value)
	{
		try
		{
			await _storageService.SetItemAsStringAsync(key, value);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { key }))
		{
			throw new UmbrellaBlazorException("There has been a problem setting the item with the specified key.", exc);
		}
	}
}