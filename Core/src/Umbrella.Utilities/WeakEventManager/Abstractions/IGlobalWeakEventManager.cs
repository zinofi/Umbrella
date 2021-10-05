namespace Umbrella.Utilities.WeakEventManager.Abstractions
{
	/// <summary>
	/// An event manager that allows subscribers to be subject to GC when still subscribed registered globally with DI.
	/// </summary>
	/// <see cref="IWeakEventManager" />
	public interface IGlobalWeakEventManager : IWeakEventManager
	{
	}
}