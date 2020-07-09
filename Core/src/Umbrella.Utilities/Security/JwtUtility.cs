using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
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
				var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

				keyValuePairs.TryGetValue(roleClaimType, out object roles);

				if (roles != null)
				{
					if (roles.ToString().Trim().StartsWith("["))
					{
						string[] parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

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
			catch (Exception exc) when (_logger.WriteError(exc, new { roleClaimType }, returnValue: true))
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

			return Convert.FromBase64String(base64);
		}
	}
}