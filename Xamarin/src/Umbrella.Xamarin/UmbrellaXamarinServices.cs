using System;
using Umbrella.Xamarin.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Umbrella.Xamarin
{
	public static class UmbrellaXamarinServices
	{
		private static IServiceProvider? _services;

		public static IServiceProvider Services
		{
			get => _services ?? throw new UmbrellaXamarinException("The IServiceProvider has not been assigned. This should be done in Startup.cs.");
			set => _services = value ?? throw new ArgumentNullException(nameof(Services));
		}

		public static T GetService<T>() => Services.GetService<T>();
	}
}