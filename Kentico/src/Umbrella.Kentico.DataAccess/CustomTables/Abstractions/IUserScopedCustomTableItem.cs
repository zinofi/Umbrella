namespace Umbrella.Kentico.DataAccess.CustomTables.Abstractions
{
	public interface IUserScopedCustomTableItem<TUserId>
	{
		TUserId UserId { get; set; }
	}
}