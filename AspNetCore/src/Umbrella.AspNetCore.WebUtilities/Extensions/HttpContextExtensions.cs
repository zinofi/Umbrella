using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression;
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

		/// <summary>
		/// Tracks an <see cref="IDataExpressionDescriptor"/> that could not be matched during the model binding process performed
		/// by types extending <see cref="DataExpressionModelBinder{TDescriptor}"/>. This is used for cases where custom sorting and
		/// filtering needs to be done.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <param name="dataExpressionDescriptor">The data expression descriptor.</param>
		public static void TrackUnmatchedDataExpressionDescriptor(this HttpContext httpContext, IDataExpressionDescriptor dataExpressionDescriptor)
		{
			List<IDataExpressionDescriptor> lstDescriptor = GetDataExpressionDescriptors(httpContext);
			lstDescriptor.Add(dataExpressionDescriptor);
		}

		/// <summary>
		/// Tracks a collection of <see cref="IDataExpressionDescriptor"/> that could not be matched during the model binding process performed
		/// by types extending <see cref="DataExpressionModelBinder{TDescriptor}"/>. This is used for cases where custom sorting and
		/// filtering needs to be done.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <param name="dataExpressionDescriptors">The data expression descriptors.</param>
		public static void TrackUnmatchedDataExpressionDescriptors(this HttpContext httpContext, IEnumerable<IDataExpressionDescriptor> dataExpressionDescriptors)
		{
			List<IDataExpressionDescriptor> lstDescriptor = GetDataExpressionDescriptors(httpContext);
			lstDescriptor.AddRange(dataExpressionDescriptors);
		}

		/// <summary>
		/// Gets all of the <see cref="IDataExpressionDescriptor"/> that could not be matched during the model binding process performed
		/// by types extending <see cref="DataExpressionModelBinder{TDescriptor}"/>. This is used for cases where custom sorting and
		/// filtering needs to be done.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <returns>A collection of the unmatched <see cref="IDataExpressionDescriptor"/> instances.</returns>
		public static IReadOnlyCollection<IDataExpressionDescriptor> GetUnmatchedDataExpressionDescriptors(this HttpContext httpContext) => GetDataExpressionDescriptors(httpContext);

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