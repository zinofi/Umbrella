using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.AspNetCore.Identity.EntityFrameworkCore;

/// <summary>
/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories.
/// </summary>
public abstract class UmbrellaIdentityDbContext : UmbrellaIdentityDbContext<IdentityUser>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaIdentityDbContext"/> class.
	/// </summary>
	/// <param name="dbContextOptions">The database context options.</param>
	/// <param name="dbContextHelper">The database context helper.</param>
	public UmbrellaIdentityDbContext(
		DbContextOptions dbContextOptions,
		IUmbrellaDbContextHelper dbContextHelper)
		: base(dbContextOptions, dbContextHelper)
	{
	}
}

/// <summary>
/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
public abstract class UmbrellaIdentityDbContext<TUser> : UmbrellaIdentityDbContext<TUser, IdentityRole, string>
	where TUser : IdentityUser
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaIdentityDbContext{TUser}"/> class.
	/// </summary>
	/// <param name="dbContextOptions">The database context options.</param>
	/// <param name="dbContextHelper">The database context helper.</param>
	public UmbrellaIdentityDbContext(
		DbContextOptions dbContextOptions,
		IUmbrellaDbContextHelper dbContextHelper)
		: base(dbContextOptions, dbContextHelper)
	{
	}
}

/// <summary>
/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
/// <typeparam name="TRole">The type of the role.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class UmbrellaIdentityDbContext<TUser, TRole, TKey> : UmbrellaIdentityDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
	where TUser : IdentityUser<TKey>
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaIdentityDbContext{TUser, TRole, TKey}"/> class.
	/// </summary>
	/// <param name="dbContextOptions">The database context options.</param>
	/// <param name="dbContextHelper">The database context helper.</param>
	public UmbrellaIdentityDbContext(
		DbContextOptions dbContextOptions,
		IUmbrellaDbContextHelper dbContextHelper)
		: base(dbContextOptions, dbContextHelper)
	{
	}
}

/// <summary>
/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
/// <typeparam name="TRole">The type of the role.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
/// <typeparam name="TUserRole">The type of the user role.</typeparam>
/// <typeparam name="TUserLogin">The type of the user login.</typeparam>
/// <typeparam name="TRoleClaim">The type of the role claim.</typeparam>
/// <typeparam name="TUserToken">The type of the user token.</typeparam>
public abstract class UmbrellaIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	where TUser : IdentityUser<TKey>
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>
	where TUserRole : IdentityUserRole<TKey>
	where TUserLogin : IdentityUserLogin<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>
	where TUserToken : IdentityUserToken<TKey>
{
	#region Protected Properties		
	/// <summary>
	/// Gets the context helper.
	/// </summary>
	protected IUmbrellaDbContextHelper ContextHelper { get; }
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaIdentityDbContext{TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/> class.
	/// </summary>
	/// <param name="dbContextHelper">The database context helper.</param>
	public UmbrellaIdentityDbContext(
		IUmbrellaDbContextHelper dbContextHelper)
	{
		ContextHelper = dbContextHelper;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaIdentityDbContext{TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/> class.
	/// </summary>
	/// <param name="dbContextOptions">The database context options.</param>
	/// <param name="dbContextHelper">The database context helper.</param>
	public UmbrellaIdentityDbContext(
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
	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => ContextHelper.SaveChangesAsync(base.SaveChangesAsync, cancellationToken);
	#endregion
}