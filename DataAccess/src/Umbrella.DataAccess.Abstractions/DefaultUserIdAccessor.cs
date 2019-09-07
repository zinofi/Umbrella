namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// A default implementation of the <see cref="ICurrentUserIdAccessor{T}"/> that essentially does nothing except return the default value for <typeparamref name="T"/>.
	/// This type is useful when working with entities that do not implement the <see cref="IUserAuditEntity{T}"/> interface.
	/// </summary>
	/// <typeparam name="T">The type used for the user id</typeparam>
	public sealed class DefaultUserIdAccessor<T> : ICurrentUserIdAccessor<T>
	{
		public T CurrentUserId => default;
	}
}