namespace Umbrella.DataAccess.Abstractions
{
	public interface IUpdatedUserAuditEntity : IUpdatedUserAuditEntity<int>
	{
	}

	public interface IUpdatedUserAuditEntity<T>
	{
		T UpdatedById { get; set; }
	}
}
