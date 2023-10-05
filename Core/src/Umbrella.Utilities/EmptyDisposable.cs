namespace Umbrella.Utilities;

/// <summary>
/// A dummy <see cref="IDisposable"/> that does nothing.
/// </summary>
/// <seealso cref="IDisposable" />
public class EmptyDisposable : IDisposable
{
	#region Public Static Properties		
	/// <summary>
	/// Gets the instance.
	/// </summary>
	public static IDisposable Instance { get; } = new EmptyDisposable();
	#endregion

	#region IDisposable Members
	/// <inheritdoc />
	public void Dispose() => GC.SuppressFinalize(this);
	#endregion
}