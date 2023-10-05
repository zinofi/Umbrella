using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.Utilities.Context;

/// <summary>
/// A default implementation of the <see cref="ICurrentUserIdAccessor{T}"/> that essentially does nothing except return the default value for <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type used for the user id</typeparam>
public sealed class DefaultUserIdAccessor<T> : ICurrentUserIdAccessor<T>
{
	/// <inheritdoc />
	public T CurrentUserId => default!;
}