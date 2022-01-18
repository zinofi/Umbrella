// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.Utilities.Hosting.Abstractions
{
	/// <summary>
	/// A base class containing core logic for hosted services.
	/// </summary>
	/// <seealso cref="IHostedService" />
	/// <seealso cref="IDisposable" />
	public abstract class UmbrellaScheduledHostedServiceBase : IHostedService, IDisposable
	{
		private CancellationTokenSource? _cancellationTokenSource;
		private readonly CrontabSchedule _crontabSchedule;
		private DateTime _nextRun;

		/// <summary>
		/// Gets the logger.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the service scope factory.
		/// </summary>
		protected IServiceScopeFactory ServiceScopeFactory { get; }

		/// <summary>
		/// Gets the synchronization manager.
		/// </summary>
		protected ISynchronizationManager SynchronizationManager { get; }

		/// <summary>
		/// Gets the crontab schedule in UTC format which determines when this services will be executed.
		/// </summary>
		/// <remarks>
		/// See https://ncrontab.swimburger.net/ to test compatible expressions.
		/// </remarks>
		protected abstract string CrontabScheduleUtcString { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaScheduledHostedServiceBase"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="serviceScopeFactory">The service scope factory.</param>
		/// <param name="synchronizationManager">The synchronization manager.</param>
		public UmbrellaScheduledHostedServiceBase(
			ILogger logger,
			IServiceScopeFactory serviceScopeFactory,
			ISynchronizationManager synchronizationManager)
		{
			Logger = logger;
			ServiceScopeFactory = serviceScopeFactory;
			SynchronizationManager = synchronizationManager;

			_crontabSchedule = CrontabSchedule.Parse(CrontabScheduleUtcString, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
			_nextRun = _crontabSchedule.GetNextOccurrence(DateTime.UtcNow);

			Logger.WriteInformation(new { _nextRun }, "First run is scheduled.");
		}

		/// <inheritdoc />
		public Task StartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

				Task.Run(async () =>
				{
					while (!cancellationToken.IsCancellationRequested)
					{
						await Task.Delay(Math.Max(0, (int)_nextRun.Subtract(DateTime.UtcNow).TotalMilliseconds));

						Logger.WriteInformation(new { Date = DateTime.UtcNow }, "Running.");

						try
						{
							using var serviceScope = ServiceScopeFactory.CreateScope();

							await ExecuteAsync(serviceScope, cancellationToken);
						}
						catch (Exception exc) when (!(exc is OperationCanceledException) && Logger.WriteError(exc, returnValue: true))
						{
							// Mask the exception to allow the job to retry on the next run
						}

						_nextRun = _crontabSchedule.GetNextOccurrence(DateTime.UtcNow);

						Logger.WriteInformation(new { _nextRun }, "Next run is scheduled.");
					}
				},
				cancellationToken);

				return Task.CompletedTask;
			}
			catch (Exception exc) when (!(exc is OperationCanceledException) && Logger.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem with this service during startup.", exc);
			}
		}

		/// <inheritdoc />
		public Task StopAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			_cancellationTokenSource?.Cancel();

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public void Dispose() => _cancellationTokenSource?.Dispose();

		/// <summary>
		/// Executes the core logic for this service.
		/// </summary>
		/// <param name="serviceScope">The service scope from which services can be resolved internally.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable <see cref="Task"/> used to await execution.</returns>
		protected abstract Task ExecuteAsync(IServiceScope serviceScope, CancellationToken cancellationToken);
	}
}