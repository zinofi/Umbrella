namespace Umbrella.DataAccess.Abstractions
{
	public interface IUserAuditEntity : IUserAuditEntity<int>
	{
	}

	// TODO: Split into 2
	public interface IUserAuditEntity<T>
	{
		T CreatedById { get; set; }
		T UpdatedById { get; set; }
	}
}