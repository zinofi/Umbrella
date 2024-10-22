using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components;
using Umbrella.AspNetCore.Shared.Extensions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.Shared.Extensions;

/// <summary>
/// Extension methods for use with the <see cref="NavigationManager" /> class.
/// </summary>
public static class NavigationManagerExtensions
{
	/// <summary>
	/// Tries the get query string value from the <see cref="NavigationManager.Uri"/> property.
	/// </summary>
	/// <typeparam name="T">The type of the value being read from the querystring.</typeparam>
	/// <param name="navManager">The nav manager.</param>
	/// <param name="key">The key of the value being read from the querystring.</param>
	/// <returns>A tuple containing fields indicating success together with any value.</returns>
	/// <exception cref="NotSupportedException">Query Paramaters of type {typeof(T).Name} cannot be converted.</exception>
	public static (bool success, T value) TryGetQueryStringValue<T>(this NavigationManager navManager, string key)
	{
		Guard.IsNotNull(navManager);

		var uri = navManager.ToAbsoluteUri(navManager.Uri);

		return uri.TryGetQueryStringValue<T>(key);
	}

	/// <summary>
	/// Tries the get query string enum value from the <see cref="NavigationManager.Uri"/> property.
	/// </summary>
	/// <typeparam name="T">The type of the enum value being read from the querystring.</typeparam>
	/// <param name="navManager">The nav manager.</param>
	/// <param name="key">The key of the value being read from the querystring.</param>
	/// <returns>A tuple containing fields indicating success together with any enum value.</returns>
	public static (bool success, T value) TryGetQueryStringEnumValue<T>(this NavigationManager navManager, string key)
		where T : struct, Enum
	{
		Guard.IsNotNull(navManager);

		var uri = navManager.ToAbsoluteUri(navManager.Uri);

		return uri.TryGetQueryStringEnumValue<T>(key);
	}
}