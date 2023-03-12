namespace Umbrella.AppFramework.Shared.Models;

/// <summary>
/// A no-op model. This exists for use when specifying generic parameters for types that make use of the AppFramework types
/// but don't support updating items.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public class NoopUpdateModel<TKey> : IUpdateModel<TKey>
	where TKey : IEquatable<TKey>
{
	/// <inheritdoc />
	public TKey Id => throw new NotImplementedException();

	/// <inheritdoc />
	public string ConcurrencyStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}