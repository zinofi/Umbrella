using System;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Contains extention methods for the <see cref="IServiceCollection"/> class for configuring Umbrella Options types.
	/// </summary>
	public static class IServiceCollectionExtensions2
	{
		/// <summary>
		/// Configures the specified Umbrella Options denoted by <typeparamref name="TOptions"/>.
		/// </summary>
		/// <typeparam name="TOptions">The type of the options.</typeparam>
		/// <param name="services">The services.</param>
		/// <param name="optionsBuilder">The options builder.</param>
		/// <returns>
		/// The same instance of <see cref="IServiceCollection"/> as passed in but with the Umbrella Options type specified by
		/// <typeparamref name="TOptions"/> added to it.
		/// </returns>
		public static IServiceCollection ConfigureUmbrellaOptions<TOptions>(this IServiceCollection services, Action<IServiceProvider, TOptions> optionsBuilder)
			where TOptions : class, new()
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton(serviceProvider =>
			{
				var options = new TOptions();
				optionsBuilder?.Invoke(serviceProvider, options);

				return options;
			});

			return services;
		}
	}
}