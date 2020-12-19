namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// This type is used to track the context of an application tenant. This should be registered with DI containers
	/// as a scoped service so that, for example, in web applications the lifetime of the context is scoped
	/// to the lifetime of the web request. In ASP.NET applications, middleware is then used to assign values to
	/// the context at the start of every request. This context can then be used throughout the application to ensure
	/// that things like access to database objects is restricted to the correct tenant.
	/// </summary>
	/// <typeparam name="TAppTenantKey">The type of the application tenant key.</typeparam>
	public class DbAppTenantSessionContext<TAppTenantKey>
	{
		/// <summary>
		/// Gets or sets the application tenant identifier.
		/// </summary>
		/// <value>
		/// The application tenant identifier.
		/// </value>
		public TAppTenantKey? AppTenantId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this tenant is authenticated.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
		/// </value>
		public bool IsAuthenticated { get; set; }
	}
}