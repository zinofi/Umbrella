using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbrella.Utilities.Data.Concurrency;

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
	}
}