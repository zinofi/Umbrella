// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.EntityFrameworkCore.SqlServer.Extensions
{
	/// <summary>
	/// Extension methods for use with <see cref="DbContext"/> instances.
	/// </summary>
	public static class DbContextExtensions
	{
		/// <summary>
		/// Gets the next integer value in the named sequence.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="sequenceName">The sequence name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public static async Task<int> GetNextIntegerSequenceValueAsync(this DbContext dbContext, string sequenceName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var parameter = new SqlParameter("@result", SqlDbType.Int)
			{
				Direction = ParameterDirection.Output
			};

			await dbContext.Database.ExecuteSqlRawAsync($"SET @result = NEXT VALUE FOR {sequenceName}", new[] { parameter }, cancellationToken);

			return (int)parameter.Value;
		}
	}
}