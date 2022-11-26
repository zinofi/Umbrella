using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EntityFrameworkCore;

/// <summary>
/// A customized <see cref="DbContext" /> class for use with the Umbrella database repositories.
/// </summary>
/// <seealso cref="DbContext" />
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
	public UmbrellaDbContext(
		IUmbrellaDbContextHelper dbContextHelper)
	{
		ContextHelper = dbContextHelper;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDbContext"/> class.
	/// </summary>
	/// <param name="dbContextOptions">The database context options.</param>
	/// <param name="dbContextHelper">The database context helper.</param>
	public UmbrellaDbContext(
		DbContextOptions dbContextOptions,
		IUmbrellaDbContextHelper dbContextHelper)
		: base(dbContextOptions)
	{
		ContextHelper = dbContextHelper;
	}
	#endregion

	#region Overridden Methods
	/// <inheritdoc />
	public override int SaveChanges() => ContextHelper.SaveChanges(base.SaveChanges);

	/// <inheritdoc />
	public override int SaveChanges(bool acceptAllChangesOnSuccess) => ContextHelper.SaveChanges(() => base.SaveChanges(acceptAllChangesOnSuccess));

	/// <inheritdoc />
	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) => ContextHelper.SaveChangesAsync(token => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken), cancellationToken);

	/// <inheritdoc />
	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken) => ContextHelper.SaveChangesAsync(base.SaveChangesAsync, cancellationToken);
	#endregion
}