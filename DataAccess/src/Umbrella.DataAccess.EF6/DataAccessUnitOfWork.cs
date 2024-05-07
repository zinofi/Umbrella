using Microsoft.Extensions.Logging;
using System.Data.Entity;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.Abstractions.Exceptions;

namespace Umbrella.DataAccess.EF6;

public class DataAccessUnitOfWork<TDbContext> : IDataAccessUnitOfWork
	where TDbContext : DbContext
{
	private readonly ILogger<DataAccessUnitOfWork<TDbContext>> _logger;
	private readonly TDbContext _dbContext;

	/// <summary>
	/// Initializes a new instance of the <see cref="DataAccessUnitOfWork{TDbContext}"/> class.
	/// </summary>
	/// <param name="logger"></param>
	/// <param name="dbContext"></param>
	public DataAccessUnitOfWork(
		ILogger<DataAccessUnitOfWork<TDbContext>> logger,
		TDbContext dbContext)
	{
		_logger = logger;
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public async Task CommitAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaDataAccessException("An error occurred committing the unit of work.", exc);
		}
	}
}