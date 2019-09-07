namespace Umbrella.DataAccess.MultiTenant.Abstractions
{
	/// <summary>
	/// This interface is used to mark a type as belonging to a specific tenant where the identifier of the tenant is an <see cref="int"/>.
	/// </summary>
	public interface IAppTenantEntity : IAppTenantEntity<int>
	{
	}

	/// <summary>
	/// This interface is used to mark a type as belonging to a specific tenant where the identifier of the tenant is of type <typeparamref name="TKey"/>.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	public interface IAppTenantEntity<TKey>
	{
		/// <summary>
		/// Gets or sets the application tenant identifier.
		/// </summary>
		/// <value>
		/// The application tenant identifier.
		/// </value>
		TKey AppTenantId { get; set; }
	}
}
