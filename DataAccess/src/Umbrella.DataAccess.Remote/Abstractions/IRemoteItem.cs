namespace Umbrella.DataAccess.Remote.Abstractions
{
	public interface IRemoteItem<TIdentifier>
	{
		TIdentifier Id { get; }
	}
}