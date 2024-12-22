using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;
using Umbrella.DataAccess.EntityFrameworkCore.SqlServer.Extensions;

namespace Umbrella.DataAccess.EntityFrameworkCore.SqlServer.Repositories;

/// <summary>
/// A repository used to access database version information for the specified <typeparamref name="TDbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <seealso cref="IDatabaseVersionRepository" />
public class DatabaseVersionRepository<TDbContext> : IDatabaseVersionRepository
	where TDbContext : DbContext
{
	private readonly ILogger _logger;
	private readonly TDbContext _context;

	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseVersionRepository{TDbContext}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="context">The context.</param>
	public DatabaseVersionRepository(
		ILogger<DatabaseVersionRepository<TDbContext>> logger,
		TDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	/// <inheritdoc />
	public async Task<string?> GetDatabaseVersionAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			return await _context.GetDatabaseVersionAsync(cancellationToken);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaDataAccessException("There has been a problem getting the database version.", exc);
		}
	}
}