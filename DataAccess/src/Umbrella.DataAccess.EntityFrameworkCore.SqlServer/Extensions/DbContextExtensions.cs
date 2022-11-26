// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Umbrella.DataAccess.EntityFrameworkCore.SqlServer.Extensions;

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
	/// <returns>The next integer value in the named sequence.</returns>
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

	/// <summary>
	/// Gets the database version based on the last applied migration.
	/// </summary>
	/// <param name="dbContext">The database context.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The current database version.</returns>
	public static async Task<string?> GetDatabaseVersionAsync(this DbContext dbContext, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var lstMigration = await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken).ConfigureAwait(false);

		string? lastMigration = lstMigration.LastOrDefault();

		if (lastMigration is not null)
		{
			int idxUnderscore = lastMigration.LastIndexOf('_');

			if (idxUnderscore >= 0)
				return lastMigration[(idxUnderscore + 1)..];
		}

		return null;
	}
}