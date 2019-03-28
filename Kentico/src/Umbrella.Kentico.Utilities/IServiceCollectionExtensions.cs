using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.Core;
using Umbrella.Kentico.Utilities.ContactManagement.Abstractions;
using Umbrella.Kentico.Utilities.ContactManagement;
using Umbrella.Kentico.Utilities.Users.Abstractions;
using Umbrella.Kentico.Utilities.Users;
using Umbrella.Kentico.Utilities.ContactManagement.Options;
using System;
using Umbrella.Kentico.Utilities.Middleware.Options;
using Umbrella.Kentico.Utilities.Middleware;
using Kentico.Activities;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaKenticoUtilities(this IServiceCollection services)
		{
			// Contact Management
			services.AddScoped(ctx => Service.Entry<IContactProcessingChecker>());
			services.AddScoped(ctx => Service.Entry<IContactCreator>());
			services.AddScoped(ctx => Service.Entry<IContactRelationAssigner>());
			services.AddScoped(ctx => Service.Entry<IContactPersistentStorage>());
			services.AddScoped(ctx => Service.Entry<IContactMergeService>());
			services.AddSingleton<IMembershipActivitiesLogger, MembershipActivitiesLogger>();

			services.AddScoped<IKenticoContactManager, KenticoContactManager>();
			services.AddSingleton<IKenticoUserNameNormalizer, KenticoUserNameNormalizer>();

			// Default Options - These can be replaced by calls to the Configure* methods below.
			services.AddSingleton<KenticoContactManagerOptions>();

			// Middleware
			services.AddScoped<MergeMarketingContactMiddleware>();

			return services;
		}

		public static IServiceCollection ConfigureKenticoContactManagerOptions(this IServiceCollection services, Action<IServiceProvider, KenticoContactManagerOptions> optionsBuilder)
			=> services.ConfigureUmbrellaOptions(optionsBuilder);

		public static IServiceCollection ConfigureMergeMarketingContactMiddlewareOptions(this IServiceCollection services, Action<IServiceProvider, MergeMarketingContactMiddlewareOptions> optionsBuilder)
			=> services.ConfigureUmbrellaOptions(optionsBuilder);
    }
}