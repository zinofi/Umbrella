using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Umbrella.DataAccess.EntityFrameworkCore.Extensions;

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
	{
		Guard.IsNotNull(builder);

		return builder.HasConversion(x => x.ToUniversalTime(), x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
	}

	/// <summary>
	/// Ensures the nullable <see cref="DateTime"/> is converted to UTC when saving and back to <see cref="DateTimeKind.Utc"/> when loaded, if a value exists.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <returns>The property builder.</returns>
	public static PropertyBuilder<DateTime?> EnsureUtc(this PropertyBuilder<DateTime?> builder)
	{
		Guard.IsNotNull(builder);

		return builder.HasConversion(x => x.HasValue ? x.Value.ToUniversalTime() : x, x => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : x);
	}

	/// <summary>
	/// Applies a precision to the specified <paramref name="builder"/> with a precision of <c>19</c> and a scale of <c>4</c>.
	/// Internally, this calls <see cref="PropertyBuilder.HasPrecision(int, int)" />.
	/// </summary>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	/// <param name="builder">The <see cref="PropertyBuilder{TProperty}"/></param>
	/// <returns>The same instance of <see cref="PropertyBuilder{TProperty}"/> as was passed into the method.</returns>
	public static PropertyBuilder<TProperty> HasCurrencyPrecisionScale4<TProperty>(this PropertyBuilder<TProperty> builder)
	{
		Guard.IsNotNull(builder);

		return builder.HasPrecision(19, 4);
	}

	/// <summary>
	/// Applies a precision to the specified <paramref name="builder"/> with a precision of <c>17</c> and a scale of <c>2</c>.
	/// Internally, this calls <see cref="PropertyBuilder.HasPrecision(int, int)" />.
	/// </summary>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	/// <param name="builder">The <see cref="PropertyBuilder{TProperty}"/></param>
	/// <returns>The same instance of <see cref="PropertyBuilder{TProperty}"/> as was passed into the method.</returns>
	public static PropertyBuilder<TProperty> HasCurrencyPrecisionScale2<TProperty>(this PropertyBuilder<TProperty> builder)
	{
		Guard.IsNotNull(builder);

		return builder.HasPrecision(17, 2);
	}
}