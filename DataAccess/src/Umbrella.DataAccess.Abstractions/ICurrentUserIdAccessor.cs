namespace Umbrella.DataAccess.Abstractions
{
	public interface ICurrentUserIdAccessor : ICurrentUserIdAccessor<int>
	{
	}

	public interface ICurrentUserIdAccessor<T>
	{
		T CurrentUserId { get; }
	}
}