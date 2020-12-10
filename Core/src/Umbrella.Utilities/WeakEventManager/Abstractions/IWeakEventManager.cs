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
		void AddEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "") where TEventHandler : Delegate;
		void RaiseEvent(object sender, string eventName, params object[] args);
		IReadOnlyCollection<TReturnValue> RaiseEvent<TReturnValue>(object sender, string eventName, params object[] args);
		void RemoveEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "") where TEventHandler : Delegate;
	}
}