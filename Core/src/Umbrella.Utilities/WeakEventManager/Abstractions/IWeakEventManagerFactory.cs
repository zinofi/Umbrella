namespace Umbrella.Utilities.WeakEventManager.Abstractions
{
	/// <summary>
	/// A factory used to create instances of <see cref="IWeakEventManager" />.
	/// </summary>
	public interface IWeakEventManagerFactory
	{
		/// <summary>
		/// Creates an instance of <see cref="IWeakEventManager"/>.
		/// </summary>
		/// <returns>The instance.</returns>
		IWeakEventManager Create();
	}
}