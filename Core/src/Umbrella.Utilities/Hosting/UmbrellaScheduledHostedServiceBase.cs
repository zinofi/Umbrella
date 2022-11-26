// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Threading.Abstractions;

namespace Umbrella.Utilities.Hosting;

/// <summary>
/// A base class containing core logic for hosted services.
/// </summary>
/// <seealso cref="IHostedService" />
/// <seealso cref="IDisposable" />
public abstract class UmbrellaScheduledHostedServiceBase : IHostedService, IDisposable
{
	private CancellationTokenSource? _cancellationTokenSource;

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
	}

	/// <inheritdoc />
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			string scheduleString = await GetCrontabScheduleUtcStringAsync(cancellationToken);
			var crontabSchedule = CrontabSchedule.Parse(scheduleString, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
			DateTime nextRun = crontabSchedule.GetNextOccurrence(DateTime.UtcNow);

			Logger.WriteInformation(new { nextRun }, "First run is scheduled.");

			_ = Task.Run(async () =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					await Task.Delay(Math.Max(0, (int)nextRun.Subtract(DateTime.UtcNow).TotalMilliseconds));

					Logger.WriteInformation(new { Date = DateTime.UtcNow }, "Running.");

					try
					{
						using var serviceScope = ServiceScopeFactory.CreateScope();

						await ExecuteAsync(serviceScope, cancellationToken);
					}
					catch (Exception exc) when (exc is not OperationCanceledException && Logger.WriteError(exc))
					{
						// Mask the exception to allow the job to retry on the next run
					}

					nextRun = crontabSchedule.GetNextOccurrence(DateTime.UtcNow);

					Logger.WriteInformation(new { nextRun }, "Next run is scheduled.");
				}
			},
			cancellationToken);
		}
		catch (Exception exc) when (exc is not OperationCanceledException && Logger.WriteError(exc))
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

	/// <summary>
	/// Gets the crontab schedule string.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The schedule string.</returns>
	protected abstract Task<string> GetCrontabScheduleUtcStringAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Executes the internal logic for this service.
	/// </summary>
	/// <param name="serviceScope">The service scope.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable <see cref="Task"/> used to await execution.</returns>
	protected internal virtual Task ExecuteInternalAsync(IServiceScope serviceScope, CancellationToken cancellationToken) => ExecuteAsync(serviceScope, cancellationToken);
}