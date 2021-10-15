using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.WeakEventManager.Abstractions;

namespace Umbrella.Utilities.WeakEventManager
{
	/// <summary>
	/// An event manager that allows subscribers to be subject to GC when still subscribed.
	/// </summary>
	/// <seealso cref="IWeakEventManager" />
	public class WeakEventManager : IWeakEventManager
	{
		// TODO: Can we use ConditionalWeakTable here instead? Would need to intern the string key
		// but would still be more memory efficient than this.
		private readonly ConcurrentDictionary<string, List<WeakEventSubscription>> _subscriptionDictionary = new ConcurrentDictionary<string, List<WeakEventSubscription>>();
		private readonly ILogger<WeakEventManager> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="WeakEventManager"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public WeakEventManager(ILogger<WeakEventManager> logger)
		{
			_logger = logger;
		}

		/// <inheritdoc />
		public void AddEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "")
			where TEventHandler : Delegate
		{
			try
			{
				var subscription = new WeakEventSubscription(new WeakReference(handler.Target), handler.Method);

				_subscriptionDictionary.AddOrUpdate(eventName, new List<WeakEventSubscription> { subscription }, (key, items) =>
				{
					items.Add(subscription);
					return items;
				});
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { eventName }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem adding the specified event handler.");
			}
		}

		/// <inheritdoc />
		public void RemoveAllEventHandlers(string eventName)
		{
			try
			{
				_subscriptionDictionary.TryRemove(eventName, out List<WeakEventSubscription> lstSubscription);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { eventName }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem removing the handlers for the specified event.");
			}
		}

		/// <inheritdoc />
		public void RemoveEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "")
			where TEventHandler : Delegate
		{
			try
			{
				if (_subscriptionDictionary.TryGetValue(eventName, out List<WeakEventSubscription> lstSubscription) && lstSubscription?.Count > 0)
				{
					var subscription = new WeakEventSubscription(new WeakReference(handler.Target), handler.Method);
					lstSubscription.RemoveAll(x => x == subscription);
				}
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { eventName }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem removing the specified event handler.");
			}
		}

		/// <inheritdoc />
		public void RaiseEvent(string eventName, params object[] args) => RaiseEvent<object>(eventName, args);

		/// <inheritdoc />
		public IReadOnlyCollection<TReturnValue> RaiseEvent<TReturnValue>(string eventName, params object[] args)
		{
			try
			{
				if (_subscriptionDictionary.TryGetValue(eventName, out List<WeakEventSubscription> lstSubscription) && lstSubscription?.Count > 0)
				{
					var lstReturnValue = new List<TReturnValue>();

					foreach (WeakEventSubscription subscription in lstSubscription)
					{
						if (subscription.Subscriber.IsAlive)
						{
							object? result = subscription.Handler.Invoke(subscription.Subscriber.Target, args);

							if (result is TReturnValue retVal)
								lstReturnValue.Add(retVal);
						}
					}

					return lstReturnValue;
				}

				return Array.Empty<TReturnValue>();
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { eventName }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem raising the specified event.");
			}
		}
	}
}