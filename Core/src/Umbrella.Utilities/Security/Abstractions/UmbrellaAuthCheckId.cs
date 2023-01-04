// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.ObjectPool;

/* Unmerged change from project 'Umbrella.Utilities(net461)'
Before:
namespace Umbrella.Utilities.Security.Abstractions
{
	/// <summary>
	/// The base class for auth check id types.
	/// </summary>
	/// <typeparam name="TAuthCheckId">The type from which this is derived.</typeparam>
	/// <typeparam name="TValue">The type of the value stored by this type.</typeparam>
	public abstract class UmbrellaAuthCheckId<TAuthCheckId, TValue> : IDisposable
		where TAuthCheckId : class
	{
		/// <summary>
		/// Gets or sets the object pool.
		/// </summary>
		public ObjectPool<TAuthCheckId>? Pool { get; internal set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public TValue Value { get; internal set; } = default!;

		/// <inheritdoc />
		public void Dispose()
		{
			if (this is TAuthCheckId value)
				Pool?.Return(value);
			else
				throw new UmbrellaException($"The object is not of type: {typeof(TAuthCheckId).FullName}.");

			Pool = null;
		}
After:
namespace Umbrella.Utilities.Security.Abstractions;

/// <summary>
/// The base class for auth check id types.
/// </summary>
/// <typeparam name="TAuthCheckId">The type from which this is derived.</typeparam>
/// <typeparam name="TValue">The type of the value stored by this type.</typeparam>
public abstract class UmbrellaAuthCheckId<TAuthCheckId, TValue> : IDisposable
	where TAuthCheckId : class
{
	/// <summary>
	/// Gets or sets the object pool.
	/// </summary>
	public ObjectPool<TAuthCheckId>? Pool { get; internal set; }

	/// <summary>
	/// Gets or sets the value.
	/// </summary>
	public TValue Value { get; internal set; } = default!;

	/// <inheritdoc />
	public void Dispose()
	{
		if (this is TAuthCheckId value)
			Pool?.Return(value);
		else
			throw new UmbrellaException($"The object is not of type: {typeof(TAuthCheckId).FullName}.");

		Pool = null;
*/
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Security.Abstractions;

/// <summary>
/// The base class for auth check id types.
/// </summary>
/// <typeparam name="TAuthCheckId">The type from which this is derived.</typeparam>
/// <typeparam name="TValue">The type of the value stored by this type.</typeparam>
public abstract class UmbrellaAuthCheckId<TAuthCheckId, TValue> : IDisposable
	where TAuthCheckId : class
{
	/// <summary>
	/// Gets or sets the object pool.
	/// </summary>
	public ObjectPool<TAuthCheckId>? Pool { get; internal set; }

	/// <summary>
	/// Gets or sets the value.
	/// </summary>
	public TValue Value { get; internal set; } = default!;

	/// <inheritdoc />
	public void Dispose()
	{
		if (this is TAuthCheckId value)
			Pool?.Return(value);
		else
			throw new UmbrellaException($"The object is not of type: {typeof(TAuthCheckId).FullName}.");

		Pool = null;
		GC.SuppressFinalize(this);
	}
}