using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EntityFrameworkCore.Extensions
{
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

		// TODO: Create a new Umbrella.DataAccess.EntityFrameworkCore.SqlServer package and move this into there.
		/// <summary>
		/// Setups the non clustered primary key.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="builder">The builder.</param>
		/// <returns>The builder.</returns>
		public static KeyBuilder SetupNonClusteredPrimaryKey<TEntity>(this EntityTypeBuilder<IEntity> builder)
			where TEntity : class, IEntity<int>
			=> builder.HasKey(x => x.Id).IsClustered(false);
	}
}