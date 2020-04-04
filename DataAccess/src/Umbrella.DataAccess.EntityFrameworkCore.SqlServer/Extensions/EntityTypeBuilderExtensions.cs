using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbrella.DataAccess.Abstractions;

namespace Umbrella.DataAccess.EntityFrameworkCore.SqlServer.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="EntityTypeBuilder{T}" /> class.
	/// </summary>
	public static class EntityTypeBuilderExtensions
	{
		/// <summary>
		/// Setups the non clustered primary key.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
		/// <param name="builder">The builder.</param>
		/// <returns>The builder.</returns>
		public static KeyBuilder SetupNonClusteredPrimaryKey<TEntity, TEntityKey>(this EntityTypeBuilder<IEntity<TEntityKey>> builder)
			where TEntity : class, IEntity<TEntityKey>
			=> builder.HasKey(x => x.Id).IsClustered(false);
	}
}