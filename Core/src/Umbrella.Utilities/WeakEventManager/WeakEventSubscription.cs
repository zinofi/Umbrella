using System;
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
		public bool Equals(WeakEventSubscription other) => Subscriber.Target.Equals(other.Subscriber.Target) && Handler.Equals(other.Handler);

		public override int GetHashCode()
		{
			int hashCode = 42314382;
			hashCode = hashCode * -1521134295 + Subscriber.Target.GetHashCode();
			hashCode = hashCode * -1521134295 + Handler.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(WeakEventSubscription left, WeakEventSubscription right) => left.Equals(right);
		public static bool operator !=(WeakEventSubscription left, WeakEventSubscription right) => !(left == right);
	}
}