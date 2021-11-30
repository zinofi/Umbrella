// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Networking.Abstractions;
using Umbrella.Xamarin.Networking;
using Umbrella.Xamarin.ObjectModel;
using Umbrella.Xamarin.ObjectModel.Abstractions;
using Umbrella.Xamarin.Utilities;
using Umbrella.Xamarin.Utilities.Abstractions;
using Umbrella.Xamarin.Utilities.Options;

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
		/// <param name="services">The services.</param>
		/// <param name="permissionsUtilityOptionsBuilder">The optional <see cref="PermissionsUtilityOptions"/> builder.</param>
		/// <returns>The services builder.</returns>
		public static IServiceCollection AddUmbrellaXamarin(
			this IServiceCollection services,
			Action<IServiceProvider, PermissionsUtilityOptions>? permissionsUtilityOptionsBuilder = null)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<IAppLocalStorageService, XamarinLocalStorageService>();
			services.AddSingleton<IDialogUtility, DialogUtility>();
			services.AddSingleton<INetworkConnectionStatusUtility, NetworkConnectionStatusUtility>();
			services.AddSingleton<IPermissionsUtility, PermissionsUtility>();
			services.AddSingleton<IUmbrellaCommandFactory, UmbrellaCommandFactory>();
			services.AddSingleton<IUriNavigator, UriNavigator>();
			services.AddSingleton<IXamarinValidationUtility, XamarinValidationUtility>();

			services.ConfigureUmbrellaOptions(permissionsUtilityOptionsBuilder);

			return services;
		}
	}
}