using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities;
using Umbrella.Xamarin.Utilities;
using Umbrella.Xamarin.Utilities.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.Xamarin"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
    {
		/// <summary>
		/// Adds the <see cref="Umbrella.Xamarin"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <returns>The services builder.</returns>
		public static IServiceCollection AddUmbrellaXamarin(this IServiceCollection services)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IAppLocalStorageService, XamarinLocalStorageService>();
			services.AddSingleton<IDialogUtility, DialogUtility>();
			services.AddSingleton<IXamarinValidationUtility, XamarinValidationUtility>();

			return services;
		}
	}
}