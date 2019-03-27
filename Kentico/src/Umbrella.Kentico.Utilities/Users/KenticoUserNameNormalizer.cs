using System;
using Microsoft.Extensions.Logging;
using Umbrella.Kentico.Utilities.Exceptions;
using Umbrella.Kentico.Utilities.Users.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.Kentico.Utilities.Users
{
	public sealed class KenticoUserNameNormalizer : IKenticoUserNameNormalizer
	{
		private readonly ILogger _log;

		public KenticoUserNameNormalizer(ILogger<KenticoUserNameNormalizer> logger)
		{
			_log = logger;
		}

		public string Normalize(string userName)
		{
			Guard.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));

			try
			{
				// NB: The Kentico API insists on user names not containg hyphens which is annoying as the Admin UI allows this!
				// Need to ensure they are always stripped out before looking up a B2C user in Kentico.
				return userName.Replace("-", "");
			}
			catch (Exception exc) when (_log.WriteError(exc, new { userName }, returnValue: true))
			{
				throw new UmbrellaKenticoException($"There has been a problem creating the Kentico UserName for the specified {nameof(userName)}.", exc);
			}
		}
	}
}