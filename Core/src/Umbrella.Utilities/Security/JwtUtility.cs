using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Security.Abstractions;

namespace Umbrella.Utilities.Security
{
	/// <summary>
	/// A utility class containing useful methods to help manage JSON Web Tokens.
	/// </summary>
	public class JwtUtility : IJwtUtility
	{
		private readonly ILogger<JwtUtility> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="JwtUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public JwtUtility(ILogger<JwtUtility> logger)
		{
			_logger = logger;
		}

		/// <inheritdoc />
		public IReadOnlyCollection<Claim> ParseClaimsFromJwt(string jwt, string roleClaimType = ClaimTypes.Role)
		{
			try
			{
				var claims = new List<Claim>();
				string payload = jwt.Split('.')[1];
				byte[] jsonBytes = ParseBase64WithoutPadding(payload);
				string json = Encoding.UTF8.GetString(jsonBytes);
				var keyValuePairs = UmbrellaStatics.DeserializeJson<Dictionary<string, object>>(json);

				if (keyValuePairs is null)
					throw new Exception("The json could not be converted to a dictionary of key/value pairs.");

				keyValuePairs.TryGetValue(roleClaimType, out object roles);

				if (roles != null)
				{
					if (roles.ToString().Trim().StartsWith("["))
					{
						string[]? parsedRoles = UmbrellaStatics.DeserializeJson<string[]>(roles.ToString());

						if (parsedRoles is null)
							throw new Exception("The roles could not be parsed.");

						foreach (string parsedRole in parsedRoles)
						{
							claims.Add(new Claim(roleClaimType, parsedRole));
						}
					}
					else
					{
						claims.Add(new Claim(roleClaimType, roles.ToString()));
					}

					keyValuePairs.Remove(roleClaimType);
				}

				claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

				return claims;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { roleClaimType }))
			{
				throw new UmbrellaException("There has been a problem parsing the claims from the JWT.", exc);
			}
		}

		private byte[] ParseBase64WithoutPadding(string base64)
		{
			base64 = (base64.Length % 4) switch
			{
				2 => base64 += "==",
				3 => base64 += "=",
				_ => base64
			};

			// Replace invalid characters.
			base64 = base64.Replace('-', '+').Replace('_', '/');

			return Convert.FromBase64String(base64);
		}
	}
}