namespace Umbrella.DataAccess.Abstractions
{
	public interface IUserAuditEntity : IUserAuditEntity<int>
	{
	}

	public interface IUserAuditEntity<T>
	{
		T CreatedById { get; set; }
		T UpdatedById { get; set; }
	}
}