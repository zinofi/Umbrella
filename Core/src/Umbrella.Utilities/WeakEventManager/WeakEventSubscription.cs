using System;
using System.Collections.Generic;
using System.Reflection;

namespace Umbrella.Utilities.WeakEventManager
{
	internal readonly struct WeakEventSubscription : IEquatable<WeakEventSubscription>
	{
		public WeakEventSubscription(WeakReference subscriber, MethodInfo handler)
		{
			Subscriber = subscriber;
			Handler = handler;
		}

		public WeakReference Subscriber { get; }
		public MethodInfo Handler { get; }

		public override bool Equals(object? obj) => obj is WeakEventSubscription subscription && Equals(subscription);
		public bool Equals(WeakEventSubscription other) => EqualityComparer<WeakReference>.Default.Equals(Subscriber, other.Subscriber) && EqualityComparer<MethodInfo>.Default.Equals(Handler, other.Handler);

		public override int GetHashCode()
		{
			int hashCode = 42314382;
			hashCode = hashCode * -1521134295 + EqualityComparer<WeakReference>.Default.GetHashCode(Subscriber);
			hashCode = hashCode * -1521134295 + EqualityComparer<MethodInfo>.Default.GetHashCode(Handler);
			return hashCode;
		}

		public static bool operator ==(WeakEventSubscription left, WeakEventSubscription right) => left.Equals(right);
		public static bool operator !=(WeakEventSubscription left, WeakEventSubscription right) => !(left == right);
	}
}