namespace Umbrella.DataAccess.Remote.Abstractions
{
	public interface IUserScopedRemoteItem<TUserId>
	{
		TUserId UserId { get; set; }
	}
}