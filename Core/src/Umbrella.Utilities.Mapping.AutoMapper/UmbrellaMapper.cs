﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Mapping.Abstractions;

namespace Umbrella.Utilities.Mapping.AutoMapper;

/// <summary>
/// A very basic implementation of the <see cref="IUmbrellaMapper"/> which uses AutoMapper internally.
/// </summary>
/// <seealso cref="IUmbrellaMapper" />
public class UmbrellaMapper : IUmbrellaMapper
{
	private readonly ILogger _logger;
	private readonly IMapper _mapper;

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaMapper"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	public UmbrellaMapper(ILogger<UmbrellaMapper> logger, IMapper mapper)
	{
		_logger = logger;
		_mapper = mapper;
	}

	/// <inheritdoc />
	public ValueTask<TDestination> MapAsync<TDestination>(object source, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			return new ValueTask<TDestination>(_mapper.Map<TDestination>(source));
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { SourceTypeName = source?.GetType().FullName }))
		{
			throw new UmbrellaMappingException("There has been a problem mapping the object.", exc);
		}
	}

	/// <inheritdoc />
	public ValueTask<IReadOnlyCollection<TDestination>> MapAsync<TDestination>(IEnumerable<object> source, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			return new ValueTask<IReadOnlyCollection<TDestination>>(_mapper.Map<IReadOnlyCollection<TDestination>>(source));
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { SourceTypeName = source?.GetType().FullName }))
		{
			throw new UmbrellaMappingException("There has been a problem mapping the object.", exc);
		}
	}

	/// <inheritdoc />
	public ValueTask<TDestination> MapAsync<TSource, TDestination>(TSource source, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			return new ValueTask<TDestination>(_mapper.Map<TSource, TDestination>(source));
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { SourceTypeName = typeof(TSource).FullName }))
		{
			throw new UmbrellaMappingException("There has been a problem mapping the object.", exc);
		}
	}

	/// <inheritdoc />
	public ValueTask<IReadOnlyCollection<TDestination>> MapAsync<TSource, TDestination>(IEnumerable<TSource> source, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			return new ValueTask<IReadOnlyCollection<TDestination>>(_mapper.Map<IEnumerable<TSource>, IReadOnlyCollection<TDestination>>(source));
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { SourceTypeName = typeof(TSource).FullName }))
		{
			throw new UmbrellaMappingException("There has been a problem mapping the object.", exc);
		}
	}

	/// <inheritdoc />
	public ValueTask<TDestination> MapAsync<TSource, TDestination>(TSource source, TDestination destination, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			return new ValueTask<TDestination>(_mapper.Map(source, destination));
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { SourceTypeName = typeof(TSource).FullName, DestinationTypeName = typeof(TDestination).FullName }))
		{
			throw new UmbrellaMappingException("There has been a problem mapping the object.", exc);
		}
	}
}