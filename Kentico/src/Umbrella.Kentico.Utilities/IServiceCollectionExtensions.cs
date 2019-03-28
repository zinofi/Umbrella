using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.Core;
using Umbrella.Kentico.Utilities.ContactManagement.Abstractions;
using Umbrella.Kentico.Utilities.ContactManagement;
using Umbrella.Kentico.Utilities.Users.Abstractions;
using Umbrella.Kentico.Utilities.Users;
using Umbrella.Kentico.Utilities.ContactManagement.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaKenticoUtilities(this IServiceCollection services)
		{
			services.AddScoped(ctx => Service.Entry<IContactProcessingChecker>());
			services.AddScoped(ctx => Service.Entry<IContactCreator>());
			services.AddScoped(ctx => Service.Entry<IContactRelationAssigner>());
			services.AddScoped(ctx => Service.Entry<IContactPersistentStorage>());
			services.AddScoped(ctx => Service.Entry<IContactMergeService>());
			services.AddScoped<IKenticoContactManager, KenticoContactManager>();
			services.AddSingleton<IKenticoUserNameNormalizer, KenticoUserNameNormalizer>();

			// Default Options - These can be replaced by calls to the Configure* methods below.
			services.AddSingleton<KenticoContactManagerOptions>();

			return services;
		}

		public static IServiceCollection ConfigureKenticoContactManagerOptions(this IServiceCollection services, Action<IServiceProvider, KenticoContactManagerOptions> optionsBuilder)
			=> services.ConfigureUmbrellaOptions(optionsBuilder);
    }
}