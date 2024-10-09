﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Umbrella.AspNetCore.Shared.Services.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Services;

/// <summary>
/// This class is used to provide access to the <see cref="CancellationToken"/> that will be signalled when the request is aborted.
/// </summary>
internal class HttpRequestAbortedService : IHttpRequestAbortedService
{
	private readonly IServiceProvider _serviceProvider;

	public HttpRequestAbortedService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public CancellationToken RequestAborted => _serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.RequestAborted ?? default;
}