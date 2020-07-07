using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Umbrella.AspNetCore.Identity.EntityFrameworkCore;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.AspNetCore.ApiAuthorization.IdentityServer
{
	/// <summary>
	/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories with Identity Server 4.
	/// </summary>
	public abstract class UmbrellaApiAuthorizationDbContext : UmbrellaApiAuthorizationDbContext<IdentityUser>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaApiAuthorizationDbContext"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="operationalStoreOptions">The operational store options.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		public UmbrellaApiAuthorizationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions,
			IUmbrellaDbContextHelper dbContextHelper)
			: base(options, operationalStoreOptions, dbContextHelper)
		{
		}
	}

	/// <summary>
	/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories with Identity Server 4.
	/// </summary>
	/// <typeparam name="TUser">The type of the user.</typeparam>
	public abstract class UmbrellaApiAuthorizationDbContext<TUser> : UmbrellaApiAuthorizationDbContext<TUser, IdentityRole, string>
		where TUser : IdentityUser
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaApiAuthorizationDbContext{TUser}"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="operationalStoreOptions">The operational store options.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		protected UmbrellaApiAuthorizationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions,
			IUmbrellaDbContextHelper dbContextHelper)
			: base(options, operationalStoreOptions, dbContextHelper)
		{
		}
	}

	/// <summary>
	/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories with Identity Server 4.
	/// </summary>
	/// <typeparam name="TUser">The type of the user.</typeparam>
	/// <typeparam name="TRole">The type of the role.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	public abstract class UmbrellaApiAuthorizationDbContext<TUser, TRole, TKey> : UmbrellaApiAuthorizationDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaApiAuthorizationDbContext{TUser, TRole, TKey}"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="operationalStoreOptions">The operational store options.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		protected UmbrellaApiAuthorizationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions,
			IUmbrellaDbContextHelper dbContextHelper)
			: base(options, operationalStoreOptions, dbContextHelper)
		{
		}
	}

	/// <summary>
	/// A customized <see cref="IdentityDbContext" /> class for use with the Umbrella database repositories with Identity Server 4.
	/// </summary>
	/// <typeparam name="TUser">The type of the user.</typeparam>
	/// <typeparam name="TRole">The type of the role.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TUserClaim">The type of the user claim.</typeparam>
	/// <typeparam name="TUserRole">The type of the user role.</typeparam>
	/// <typeparam name="TUserLogin">The type of the user login.</typeparam>
	/// <typeparam name="TRoleClaim">The type of the role claim.</typeparam>
	/// <typeparam name="TUserToken">The type of the user token.</typeparam>
	public abstract class UmbrellaApiAuthorizationDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : UmbrellaIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IPersistedGrantDbContext
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserClaim : IdentityUserClaim<TKey>
		where TUserRole : IdentityUserRole<TKey>
		where TUserLogin : IdentityUserLogin<TKey>
		where TRoleClaim : IdentityRoleClaim<TKey>
		where TUserToken : IdentityUserToken<TKey>
	{
		/// <summary>
		/// Gets the operational store options.
		/// </summary>
		protected IOptions<OperationalStoreOptions> OperationalStoreOptions { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaApiAuthorizationDbContext{TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="operationalStoreOptions">The operational store options.</param>
		/// <param name="dbContextHelper">The database context helper.</param>
		public UmbrellaApiAuthorizationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions,
			IUmbrellaDbContextHelper dbContextHelper)
			: base(options, dbContextHelper)
		{
			OperationalStoreOptions = operationalStoreOptions;
		}

		/// <inheritdoc />
		public DbSet<PersistedGrant> PersistedGrants { get; set; } = null!;

		/// <inheritdoc />
		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; } = null!;

		Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

		/// <inheritdoc />
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ConfigurePersistedGrantContext(OperationalStoreOptions.Value);
		}
	}
}