namespace Umbrella.DataAccess.Abstractions
{
	public interface ICreatedUserAuditEntity : IUpdatedUserAuditEntity<int>
	{
	}

	public interface ICreatedUserAuditEntity<T>
	{
		T CreatedById { get; set; }
	}
}