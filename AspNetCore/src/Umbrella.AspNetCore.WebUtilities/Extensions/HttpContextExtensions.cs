using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.WebUtilities.Security;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="HttpContext"/> class.
	/// </summary>
	public static class HttpContextExtensions
	{
		private const string UnmatchedDataExpressionDescriptorKey = nameof(UnmatchedDataExpressionDescriptorKey);

		/// <summary>
		/// Gets the value of the <see cref="NonceContext.Current"/> property stored in the <see cref="NonceContext"/> in the <see cref="HttpContext.Features"/> collection.
		/// This needs to be manually added first as part of the current request, typically using middleware.
		/// </summary>
		/// <param name="httpContext">The current HTTP context.</param>
		/// <returns>The value of <see cref="NonceContext.Current"/> stored on the current HTTP context.</returns>
		public static string? GetCurrentRequestNonce(this HttpContext httpContext) => httpContext.Features.Get<NonceContext>()?.Current;

		public static void TrackUnmatchedDataExpressionDescriptor(this HttpContext httpContext, IDataExpressionDescriptor dataExpressionDescriptor)
		{
			List<IDataExpressionDescriptor> lstDescriptor = GetDataExpressionDescriptors(httpContext);
			lstDescriptor.Add(dataExpressionDescriptor);
		}

		public static void TrackUnmatchedDataExpressionDescriptors(this HttpContext httpContext, IEnumerable<IDataExpressionDescriptor> dataExpressionDescriptors)
		{
			List<IDataExpressionDescriptor> lstDescriptor = GetDataExpressionDescriptors(httpContext);
			lstDescriptor.AddRange(dataExpressionDescriptors);
		}

		public static void GetUnmatchedDataExpressionDescriptors(this HttpContext httpContext) => GetDataExpressionDescriptors(httpContext);

		private static List<IDataExpressionDescriptor> GetDataExpressionDescriptors(HttpContext httpContext)
		{
			if (!httpContext.Items.TryGetValue(UnmatchedDataExpressionDescriptorKey, out object? objValue) || !(objValue is List<IDataExpressionDescriptor> lstDescriptor))
			{
				lstDescriptor = new List<IDataExpressionDescriptor>();
				httpContext.Items[UnmatchedDataExpressionDescriptorKey] = lstDescriptor;
			}

			return lstDescriptor;
		}
	}
}