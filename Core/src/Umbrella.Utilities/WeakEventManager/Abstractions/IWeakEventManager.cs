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
		/// <param name="state">Optional state that will be passed in as the first parameter when the <paramref name="handler"/> is invoked if the <paramref name="state"/> parameter is not null; otherwise it will not be passed in.</param>
		/// <remarks>
		/// Because this uses <see cref="WeakReference"/> internally, be careful when using lambda expressions for the handler because they can be subject to GC if they
		/// produce closures, i.e. Display Classes. This can be avoided by ensuring the variables captured by any lambda expressions, including nested ones, only ever
		/// capture 'this'.
		/// </remarks>
		void AddEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "", Dictionary<string, object>? state = null) where TEventHandler : Delegate;

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
		void RemoveEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "") where TEventHandler : Delegate;
	}
}