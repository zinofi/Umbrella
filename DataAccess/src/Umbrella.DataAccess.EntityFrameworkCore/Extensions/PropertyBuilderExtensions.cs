using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Umbrella.DataAccess.EntityFrameworkCore.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="PropertyBuilder{T}" /> class.
	/// </summary>
	public static class PropertyBuilderExtensions
	{
		/// <summary>
		/// Ensures the <see cref="DateTime"/> is converted to UTC when saving and back to <see cref="DateTimeKind.Utc"/> when loaded.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>The property builder.</returns>
		public static PropertyBuilder<DateTime> EnsureUtc(this PropertyBuilder<DateTime> builder)
			=> builder.HasConversion(x => x.ToUniversalTime(), x => DateTime.SpecifyKind(x, DateTimeKind.Utc));

		/// <summary>
		/// Ensures the nullable <see cref="DateTime"/> is converted to UTC when saving and back to <see cref="DateTimeKind.Utc"/> when loaded, if a value exists.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>The property builder.</returns>
		public static PropertyBuilder<DateTime?> EnsureUtc(this PropertyBuilder<DateTime?> builder)
			=> builder.HasConversion(x => x.HasValue ? x.Value.ToUniversalTime() : x, x => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : x);
	}
}