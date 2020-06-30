using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EF6
{
	/// <summary>
	/// A customized <see cref="DbContext" /> class for use with the Umbrella database repositories.
	/// </summary>
	/// <seealso cref="System.Data.Entity.DbContext" />
	public abstract class UmbrellaDbContext : DbContext
	{
		#region Protected Properties
		/// <summary>
		/// Gets the context helper.
		/// </summary>
		protected IUmbrellaDbContextHelper ContextHelper { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDbContext"/> class.
		/// </summary>
		/// <param name="dbContextHelper">The database context helper.</param>
		public UmbrellaDbContext(IUmbrellaDbContextHelper dbContextHelper)
		{
			ContextHelper = dbContextHelper;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaDbContext"/> class.
		/// </summary>
		/// <param name="dbContextHelper">The database context helper.</param>
		/// <param name="nameOrConnectionString">The name or connection string.</param>
		public UmbrellaDbContext(
			IUmbrellaDbContextHelper dbContextHelper,
			string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
			ContextHelper = dbContextHelper;
		}
		#endregion


		#region Overridden Methods
		/// <inheritdoc />
		public override int SaveChanges() => ContextHelper.SaveChanges(base.SaveChanges);

		/// <inheritdoc />
		public override Task<int> SaveChangesAsync() => ContextHelper.SaveChangesAsync(base.SaveChangesAsync);

		/// <inheritdoc />
		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken) => ContextHelper.SaveChangesAsync(base.SaveChangesAsync, cancellationToken);
		#endregion
	}
}