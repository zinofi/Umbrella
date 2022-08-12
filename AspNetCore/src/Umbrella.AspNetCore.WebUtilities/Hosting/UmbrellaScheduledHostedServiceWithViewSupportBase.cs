// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Umbrella.AspNetCore.WebUtilities.Hosting.Options;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Hosting
{
	/// <summary>
	/// A base class containing core logic for hosted services with support for MVC views.
	/// </summary>
	/// <remarks>
	/// Internally, this creates a new <see cref="DefaultHttpContext"/> and assigns it to <see cref="IHttpContextAccessor.HttpContext"/>.
	/// If additional customization is required, the <see cref="IHttpContextAccessor"/> can be retrieved from the <see cref="IServiceScope"/>
	/// inside the <see cref="UmbrellaScheduledHostedServiceBase.ExecuteAsync(IServiceScope, CancellationToken)"/> method.
	/// </remarks>
	/// <seealso cref="UmbrellaScheduledHostedServiceBase" />
	public abstract class UmbrellaScheduledHostedServiceWithViewSupportBase : UmbrellaScheduledHostedServiceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaScheduledHostedServiceWithViewSupportBase"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="serviceScopeFactory">The service scope factory.</param>
		/// <param name="synchronizationManager">The synchronization manager.</param>
		/// <param name="options">The options.</param>
		protected UmbrellaScheduledHostedServiceWithViewSupportBase(
			ILogger<UmbrellaScheduledHostedServiceWithViewSupportBase> logger,
			IServiceScopeFactory serviceScopeFactory,
			ISynchronizationManager synchronizationManager,
			UmbrellaScheduledHostedServiceWithViewSupportOptions options)
			: base(logger, serviceScopeFactory, synchronizationManager)
		{
			Options = options;
		}

		/// <summary>
		/// Gets the options.
		/// </summary>
		protected UmbrellaScheduledHostedServiceWithViewSupportOptions Options { get; }

		/// <inheritdoc/>
		protected internal override Task ExecuteInternalAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var httpContext = new DefaultHttpContext
			{
				RequestServices = serviceScope.ServiceProvider
			};

			httpContext.Request.Scheme = Options.ContentUrlScheme;
			httpContext.Request.Host = new HostString(Options.ContentUrlHost);
			httpContext.Request.Headers.Add("Accept-Language", Options.DefaultLanguageCultureCode);

			// We need to manually set these values to ensure correct formatting of values inside views.
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(Options.DefaultLanguageCultureCode);
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Options.DefaultLanguageUICultureCode);

			serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = httpContext;

			return ExecuteAsync(serviceScope, cancellationToken);
		}
	}
}