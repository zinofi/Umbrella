// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities.Data.Concurrency;

namespace Umbrella.DataAccess.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for the <see cref="EntityTypeBuilder{T}" /> class.
/// </summary>
public static class EntityTypeBuilderExtensions
{
	/// <summary>
	/// Setups the concurrency token.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <returns>The builder.</returns>
	public static PropertyBuilder<string> SetupConcurrencyToken<TEntity>(this EntityTypeBuilder<TEntity> builder)
		where TEntity : class, IConcurrencyStamp
		=> builder.Property(x => x.ConcurrencyStamp).HasMaxLength(36).IsRequired().IsConcurrencyToken();

	public static PropertyBuilder<DateTime> SetupCreatedDateUtcAuditProperty<TEntity>(this EntityTypeBuilder<TEntity> builder)
		where TEntity : class, ICreatedDateAuditEntity
		=> builder.Property(x => x.CreatedDateUtc).EnsureUtc();

	public static PropertyBuilder<DateTime> SetupUpdatedDateUtcAuditProperty<TEntity>(this EntityTypeBuilder<TEntity> builder)
		where TEntity : class, IUpdatedDateAuditEntity
		=> builder.Property(x => x.UpdatedDateUtc).EnsureUtc();

	public static ReferenceCollectionBuilder<TAppUserEntity, TEntity> SetupCreatedByIdAuditProperty<TEntity, TAppUserEntity, TUserId>(this EntityTypeBuilder<TEntity> builder)
		where TEntity : class, ICreatedUserAuditEntity<TUserId>
		where TAppUserEntity : class
		=> builder.HasOne<TAppUserEntity>().WithMany().HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);

	public static ReferenceCollectionBuilder<TAppUserEntity, TEntity> SetupUpdatedByIdAuditProperty<TEntity, TAppUserEntity, TUserId>(this EntityTypeBuilder<TEntity> builder)
		where TEntity : class, IUpdatedUserAuditEntity<TUserId>
		where TAppUserEntity : class
		=> builder.HasOne<TAppUserEntity>().WithMany().HasForeignKey(x => x.UpdatedById).OnDelete(DeleteBehavior.Restrict);

	public static EntityTypeBuilder<TEntity> SetupAuditProperties<TEntity, TAppUserEntity, TUserId>(this EntityTypeBuilder<TEntity> builder)
		where TEntity : class, ICreatedDateAuditEntity, IUpdatedDateAuditEntity, ICreatedUserAuditEntity<TUserId>, IUpdatedUserAuditEntity<TUserId>
		where TAppUserEntity : class
	{
		_ = SetupCreatedDateUtcAuditProperty(builder);
		_ = SetupUpdatedDateUtcAuditProperty(builder);
		_ = SetupCreatedByIdAuditProperty<TEntity, TAppUserEntity, TUserId>(builder);
		_ = SetupUpdatedByIdAuditProperty<TEntity, TAppUserEntity, TUserId>(builder);

		return builder;
	}
}