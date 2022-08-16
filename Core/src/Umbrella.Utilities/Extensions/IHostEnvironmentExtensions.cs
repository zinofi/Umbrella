// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Hosting;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="IHostEnvironment"/>.
	/// </summary>
	public static class IHostEnvironmentExtensions
	{
		/// <summary>
		/// Checks if the current host environment name is <c>Integration</c>
		/// </summary>
		/// <param name="hostEnvironment">The host environment.</param>
		/// <returns>
		///   <see langword="true"/> if the specified host environment is <c>Integration</c>; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsIntegration(this IHostEnvironment hostEnvironment) => hostEnvironment.IsEnvironment("Integration");

		/// <summary>
		/// Checks if the current host environment name is <c>PreProduction</c>
		/// </summary>
		/// <param name="hostEnvironment">The host environment.</param>
		/// <returns>
		///   <see langword="true"/> if the specified host environment is <c>PreProduction</c>; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsPreProduction(this IHostEnvironment hostEnvironment) => hostEnvironment.IsEnvironment("PreProduction");
	}
}