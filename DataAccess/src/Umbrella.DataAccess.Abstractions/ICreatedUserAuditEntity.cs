namespace Umbrella.DataAccess.Abstractions
{
	public interface ICreatedUserAuditEntity : ICreatedUserAuditEntity<int>
	{
	}

	public interface ICreatedUserAuditEntity<T>
	{
		T CreatedById { get; set; }
	}
}