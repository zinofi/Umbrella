using Umbrella.AspNetCore.WebUtilities.FileSystem.Middleware;
using Umbrella.Utilities;

#pragma warning disable IDE0130
namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// Extension methods used to register Middleware for the <see cref="Umbrella.AspNetCore.WebUtilities.FileSystem"/> package with a specified <see cref="IApplicationBuilder"/>.
	/// </summary>
	public static class IApplicationBuilderExtensions
	{
		/// <summary>
		/// Add the <see cref="FileSystemMiddleware"/> to the pipeline.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>The application builder.</returns>
		public static IApplicationBuilder UseUmbrellaFileSystem(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			builder.UseMiddleware<FileSystemMiddleware>();

			return builder;
		}
	}
}