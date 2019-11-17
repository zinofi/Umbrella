namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// A default implementation of the <see cref="ICurrentUserIdAccessor"/> that essentially does nothing except return the default value for <see langword="int"/>, i.e. zero.
	/// This type is useful when working with entities that do not implement either <see cref="ICreatedUserAuditEntity"/> or <see cref="IUpdatedUserAuditEntity"/>.
	/// </summary>
	public class DefaultUserIdAccessor : DefaultUserIdAccessor<int>, ICurrentUserIdAccessor
	{
	}

	/// <summary>
	/// A default implementation of the <see cref="ICurrentUserIdAccessor{T}"/> that essentially does nothing except return the default value for <typeparamref name="T"/>.
	/// This type is useful when working with entities that do not implement either <see cref="ICreatedUserAuditEntity{T}"/> or <see cref="IUpdatedUserAuditEntity{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type used for the user id</typeparam>
	public class DefaultUserIdAccessor<T> : ICurrentUserIdAccessor<T>
	{
		/// <summary>
		/// Gets the current user identifier.
		/// </summary>
		public T CurrentUserId => default;
	}
}