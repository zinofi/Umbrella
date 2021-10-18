using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Umbrella.Utilities.WeakEventManager.Abstractions
{
	/// <summary>
	/// An event manager that allows subscribers to be subject to GC when still subscribed.
	/// </summary>
	public interface IWeakEventManager
	{
		/// <summary>
		/// Adds the event <paramref name="handler"/> using the specified <paramref name="eventName"/>.
		/// </summary>
		/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
		/// <param name="handler">The handler.</param>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="targetOverride">An override to use instead of <see cref="Delegate.Target"/> from the <paramref name="handler"/> parameter.</param>
		void AddEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "", object? targetOverride = null) where TEventHandler : Delegate;

		/// <summary>
		/// Raises the event with the specified <paramref name="eventName"/>.
		/// If no valid event handlers exists for the named event, nothing will happen.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="args">The arguments.</param>
		void RaiseEvent(string eventName, params object[] args);

		/// <summary>
		/// Raises the event with the specified <paramref name="eventName"/>.
		/// If no valid event handlers exists for the named event, nothing will happen.
		/// </summary>
		/// <typeparam name="TReturnValue">The type of the return value.</typeparam>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A collection of return values that have been output as the result of calling the registered handlers for the named event.</returns>
		IReadOnlyCollection<TReturnValue> RaiseEvent<TReturnValue>(string eventName, params object[] args);
		
		/// <summary>
		/// Removes all event handlers registered with the specified <paramref name="eventName"/>.
		/// </summary>
		/// <param name="eventName">The name of the event.</param>
		void RemoveAllEventHandlers(string eventName);

		/// <summary>
		/// Removes the event <paramref name="handler"/> registered with the specified <paramref name="eventName"/>.
		/// </summary>
		/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
		/// <param name="handler">The handler.</param>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="targetOverride">An override to use instead of <see cref="Delegate.Target"/> from the <paramref name="handler"/> parameter.</param>
		void RemoveEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "", object? targetOverride = null) where TEventHandler : Delegate;
	}
}