using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.WeakEventManager.Abstractions;

namespace Umbrella.Utilities.WeakEventManager
{
	/// <summary>
	/// A factory used to create instances of <see cref="IWeakEventManager" />.
	/// </summary>
	/// <seealso cref="IWeakEventManagerFactory"/>
	public class WeakEventManagerFactory : IWeakEventManagerFactory
	{
		private readonly ILogger _logger;
		private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="WeakEventManagerFactory"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="serviceProvider">The service provider.</param>
		public WeakEventManagerFactory(
			ILogger logger,
			IServiceProvider serviceProvider)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
		}

		/// <inheritdoc />
		public IWeakEventManager Create()
		{
			try
			{
				return ActivatorUtilities.CreateInstance<WeakEventManager>(_serviceProvider);
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem creating an instance of WeakEventManager.", exc);
			}
		}
	}
}