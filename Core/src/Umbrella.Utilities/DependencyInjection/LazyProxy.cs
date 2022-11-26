﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.DependencyInjection;


/* Unmerged change from project 'Umbrella.Utilities(net461)'
Before:
namespace Umbrella.Utilities.DependencyInjection
{
	/// <summary>
	/// An extension of the Lazy class indended to support allowing Lazy types
	/// to be injected into constructors. Look at <see cref="IServiceCollectionExtensions"/> to see it's service registration.
	/// </summary>
	/// <typeparam name="T">The type that Lazy wraps around.</typeparam>
	/// <seealso cref="System.Lazy{T}" />
	internal class LazyProxy<T> : Lazy<T>
		where T : class
	{
		public LazyProxy(IServiceProvider serviceProvider)
			: base(() => serviceProvider.GetRequiredService<T>())
		{
		}
After:
namespace Umbrella.Utilities.DependencyInjection;

/// <summary>
/// An extension of the Lazy class indended to support allowing Lazy types
/// to be injected into constructors. Look at <see cref="IServiceCollectionExtensions"/> to see it's service registration.
/// </summary>
/// <typeparam name="T">The type that Lazy wraps around.</typeparam>
/// <seealso cref="Lazy{T}" />
internal class LazyProxy<T> : Lazy<T>
	where T : class
{
	public LazyProxy(IServiceProvider serviceProvider)
		: base(() => serviceProvider.GetRequiredService<T>())
	{
*/
namespace Umbrella.Utilities.DependencyInjection;

/// <summary>
/// An extension of the Lazy class indended to support allowing Lazy types
/// to be injected into constructors. Look at <see cref="IServiceCollectionExtensions"/> to see it's service registration.
/// </summary>
/// <typeparam name="T">The type that Lazy wraps around.</typeparam>
/// <seealso cref="Lazy{T}" />
internal class LazyProxy<T> : Lazy<T>
	where T : class
{
	public LazyProxy(IServiceProvider serviceProvider)
		: base(() => serviceProvider.GetRequiredService<T>())
	{
	}
}