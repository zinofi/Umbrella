using System.Web;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
    /// <summary>
    /// Contains extension methods for the <see cref="HttpResponseBase"/> type.
    /// </summary>
    public static class HttpResponseBaseExtensions
    {
        /// <summary>
        /// Determine whether the <see cref="HttpResponseBase"/> has a success code between 200 and 299 inclusive.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseBase"/> to check.</param>
        /// <returns>True if the response status code was between 200 and 299 inclusive. Otherwise false.</returns>
        public static bool IsSuccessStatusCode(this HttpResponseBase response) => response.StatusCode >= 200 && response.StatusCode <= 299;
    }
}