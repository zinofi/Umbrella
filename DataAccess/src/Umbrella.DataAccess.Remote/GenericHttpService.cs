using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Remote.Exceptions;

namespace Umbrella.DataAccess.Remote
{
    public class GenericHttpService
    {
		#region Protected Constants
		protected const string UnauthorizedErrorMessage = "You need to be logged in to perform the current action.";
		protected const string ForbiddenErrorMessage = "You are not permitted to access the requested resource.";
		protected const string ServerErrorMessage = "An error has occurred on the remote server. Please try again.";
		protected const string UnknownErrorMessage = "An unknown error has occurred. Please try again.";
		#endregion

		#region Protected Static Properties
		protected static HttpMethod PatchHttpMethod { get; } = new HttpMethod("PATCH");
		#endregion

		#region Protected Properties
		protected ILogger Log { get; }
		protected HttpClient Client { get; }
		#endregion

		#region Constructors
		public GenericHttpService(
			ILogger logger,
			HttpClient client)
		{
			Log = logger;
			Client = client;
		}
		#endregion

		#region Protected Methods
		protected UmbrellaHttpServiceAccessException CreateServiceAccessException(Exception exception)
		{
			// If we already have an exception of the requested type just return it
			if (exception is UmbrellaHttpServiceAccessException)
				return exception as UmbrellaHttpServiceAccessException;

			return new UmbrellaHttpServiceAccessException(UnknownErrorMessage, exception);
		}

		protected void LogUnknownError(string url, string errorMessage)
			=> Log.LogError($"There was a problem accessing the {url} endpoint. The error from the server was: {errorMessage}");
		#endregion
	}
}