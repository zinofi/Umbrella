using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Umbrella.AspNetCore.WebUtilities.Hosting.Options;
using Umbrella.Utilities.Dating.Abstractions;
using Umbrella.Utilities.Hosting;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Hosting;

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
	/// <param name="dateTimeProvider">The date time provider.</param>
	/// <param name="options">The options.</param>
	protected UmbrellaScheduledHostedServiceWithViewSupportBase(
		ILogger<UmbrellaScheduledHostedServiceWithViewSupportBase> logger,
		IServiceScopeFactory serviceScopeFactory,
		ISynchronizationManager synchronizationManager,
		IDateTimeProvider dateTimeProvider,
		UmbrellaScheduledHostedServiceWithViewSupportOptions options)
		: base(logger, serviceScopeFactory, synchronizationManager, dateTimeProvider)
	{
		Options = options;
	}

	/// <summary>
	/// Gets the options.
	/// </summary>
	protected UmbrellaScheduledHostedServiceWithViewSupportOptions Options { get; }

	/// <summary>
	/// Gets the current user.
	/// </summary>
	/// <remarks>
	/// This defaults to <see langword="null" />. Override this property to assign a custom <see cref="ClaimsPrincipal" />
	/// to assign to the <see cref="HttpContext" /> created when this job is executed.
	/// </remarks>
	protected virtual ClaimsPrincipal? CurrentUser { get; }

	/// <inheritdoc/>
	private protected override async Task ExecuteInternalAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var httpContext = new DefaultHttpContext
		{
			RequestServices = serviceScope.ServiceProvider
		};

		httpContext.Request.Scheme = Options.ContentUrlScheme;
		httpContext.Request.Host = new HostString(Options.ContentUrlHost);
		httpContext.Request.Headers.AcceptLanguage = new StringValues(Options.DefaultLanguageCultureCode);

		// We need to manually set these values to ensure correct formatting of values inside views.
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(Options.DefaultLanguageCultureCode);
		Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Options.DefaultLanguageUICultureCode);

		if (CurrentUser is not null)
			httpContext.User = CurrentUser;

		serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = httpContext;

		await ExecuteAsync(serviceScope, cancellationToken).ConfigureAwait(false);
	}
}