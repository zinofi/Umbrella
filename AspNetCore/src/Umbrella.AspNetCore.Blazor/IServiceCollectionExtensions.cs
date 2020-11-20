using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using Tewr.Blazor.FileReader;
using Umbrella.AppFramework.Security;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AspNetCore.Blazor.Components.Dialog;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Utilities;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.AspNetCore.Blazor"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
    {
		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.Blazor"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <returns>The services builder.</returns>
		public static IServiceCollection AddUmbrellaAspNetCoreComponents<TAuthHelper>(this IServiceCollection services)
			where TAuthHelper : class, IAppAuthHelper
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddBlazoredLocalStorage();
			services.AddBlazoredSessionStorage();
			services.AddBlazoredModal();
			services.AddFileReaderService(x => x.UseWasmSharedBuffer = true);

			services.AddScoped<IAppLocalStorageService, BlazorLocalStorageService>();
			services.AddScoped<IUmbrellaDialogUtility, UmbrellaDialogUtility>();
			services.AddTransient<IDialogUtility>(x => x.GetService<IUmbrellaDialogUtility>());

			return services;
		}
    }
}