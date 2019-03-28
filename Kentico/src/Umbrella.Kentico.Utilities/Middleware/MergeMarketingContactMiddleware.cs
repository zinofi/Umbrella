using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kentico.Activities;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Umbrella.Kentico.Utilities.ContactManagement.Abstractions;
using Umbrella.Kentico.Utilities.Middleware.Options;
using Umbrella.Kentico.Utilities.Users.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.Kentico.Utilities.Middleware
{
	public class MergeMarketingContactMiddleware : OwinMiddleware
	{
		private readonly ILogger _log;
		private readonly MergeMarketingContactMiddlewareOptions _options;
		private readonly Lazy<IKenticoContactManager> _kenticoContactManager;
		private readonly IKenticoUserNameNormalizer _kenticoUserNameNormalizer;
		private readonly IMembershipActivitiesLogger _membershipActivitiesLogger;

		public MergeMarketingContactMiddleware(
			ILogger<MergeMarketingContactMiddleware> logger,
			MergeMarketingContactMiddlewareOptions options,
			Lazy<IKenticoContactManager> kenticoContactManager,
			IKenticoUserNameNormalizer kenticoUserNameNormalizer,
			IMembershipActivitiesLogger membershipActivitiesLogger,
			OwinMiddleware next)
			: base(next)
		{
			Guard.ArgumentNotNullOrWhiteSpace(options.IsSigningInClaimType, nameof(options.IsSigningInClaimType));
			Guard.ArgumentNotNullOrWhiteSpace(options.KenticoSiteName, nameof(options.KenticoSiteName));
			Guard.ArgumentNotNull(options.ShouldExecuteForPathDeterminer, nameof(options.ShouldExecuteForPathDeterminer));

			_log = logger;
			_options = options;
			_kenticoContactManager = kenticoContactManager;
			_kenticoUserNameNormalizer = kenticoUserNameNormalizer;
			_membershipActivitiesLogger = membershipActivitiesLogger;
		}

		public override async Task Invoke(IOwinContext context)
		{
			context.Request.CallCancelled.ThrowIfCancellationRequested();

			try
			{
				await Next.Invoke(context);

				if (context.Request.User.Identity.IsAuthenticated)
				{
					string path = context.Request.Path.Value;

					if (_options.ShouldExecuteForPathDeterminer(path))
					{
						CookieOptions CreateCookieOptions() => new CookieOptions
						{
							Domain = _options.CookieDomain, // Needs to be the same value as the Auth cookie
							Expires = DateTime.UtcNow.Add(_options.CookieExpiration),
							HttpOnly = true,
							Secure = _options.CookieSecure
						};

						// Check if the user is signing-in as part of this request
						var claimsIdentity = (ClaimsIdentity)context.Request.User.Identity;

						Claim claim = claimsIdentity.FindFirst(_options.IsSigningInClaimType);

						if (claim != null && claim.ValueType == ClaimValueTypes.Boolean && bool.TryParse(claim.Value, out bool isSigningIn) && isSigningIn)
						{
							// Merge the marketing contact
							_kenticoContactManager.Value.ContingentMerge(context, _options.KenticoSiteName, true, CreateCookieOptions);

							if (_options.LogLoginActivity)
							{
								// Now that the contact data has been merged correctly, we can log an activity against the user
								string kenticoUserName = _kenticoUserNameNormalizer.Normalize(context.Request.User.Identity.Name);
								_membershipActivitiesLogger.LogLoginActivity(kenticoUserName);
							}

							// Remove the claim now we no longer need it
							claimsIdentity.RemoveClaim(claim);

							// Calling SignIn to issue a new Auth cookie with updated claims
							context.Authentication.SignIn(claimsIdentity);
						}
						else
						{
							// Merge the marketing contact
							_kenticoContactManager.Value.ContingentMerge(context, _options.KenticoSiteName, false, CreateCookieOptions);
						}
					}
				}
			}
			catch (Exception exc) when (_log.WriteError(exc, new { Path = context.Request.Path.Value }))
			{
				throw;
			}
		}
	}
}