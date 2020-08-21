namespace Umbrella.Utilities.Http.Constants
{
	/// <summary>
	/// Contains default error messages for use with HTTP services.
	/// </summary>
	public static class HttpServiceMessages
	{
		/// <summary>
		/// The unauthorized error message
		/// </summary>
		public const string DefaultUnauthorizedErrorMessage = "You need to be logged in to perform the current action.";

		/// <summary>
		/// The forbidden error message
		/// </summary>
		public const string DefaultForbiddenErrorMessage = "You are not permitted to access the requested resource.";

		/// <summary>
		/// The server error message
		/// </summary>
		public const string DefaultServerErrorMessage = "An error has occurred on the remote server. Please try again.";

		/// <summary>
		/// The unknown error message
		/// </summary>
		public const string DefaultUnknownErrorMessage = "An unknown error has occurred. Please try again.";
	}
}