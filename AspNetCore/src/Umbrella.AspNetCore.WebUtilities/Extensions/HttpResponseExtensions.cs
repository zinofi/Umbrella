using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Contains extension methods for the <see cref="HttpResponse"/> type.
	/// </summary>
	public static class HttpResponseExtensions
	{
		/// <summary>
		/// Sends the specified status code.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="sendNullBody">if set to <see langword="true"/>, sets <see cref="HttpResponse.Body" /> to <see cref="Stream.Null"/>.</param>
		public static void SendStatusCode(this HttpResponse response, HttpStatusCode statusCode, bool sendNullBody = true)
		{
			response.StatusCode = (int)statusCode;

			if (sendNullBody)
			{
				response.Body.Close();
				response.Body = Stream.Null;
			}
		}

		/// <summary>
		/// Determine whether the <see cref="HttpResponse"/> has a success code between 200 and 299 inclusive.
		/// </summary>
		/// <param name="response">The <see cref="HttpResponse"/> to check.</param>
		/// <returns><see langword="true"/> if the response status code was between 200 and 299 inclusive. Otherwise <see langword="false"/>.</returns>
		public static bool IsSuccessStatusCode(this HttpResponse response) => response.StatusCode >= 200 && response.StatusCode <= 299;
	}
}