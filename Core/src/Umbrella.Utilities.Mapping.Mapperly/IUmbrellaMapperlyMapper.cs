// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.Utilities.Mapping.Mapperly;

/// <summary>
/// Specifies the contract for a mapperly mapping class.
/// </summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TDestination">The type of the destination.</typeparam>
public interface IUmbrellaMapperlyMapper<TSource, TDestination>
{
	/// <summary>
	/// Maps the specified <paramref name="source"/> to the <typeparamref name="TDestination"/> type.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <returns>The mapped destination type.</returns>
	TDestination Map(TSource source);

	/// <summary>
	/// Maps all of the specified <paramref name="source"/> items to a collection of <typeparamref name="TDestination"/> items.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <returns>The mapped <typeparamref name="TDestination"/> item collection.</returns>
	IReadOnlyCollection<TDestination> MapAll(IEnumerable<TSource> source);

	/// <summary>
	/// Maps the specified <paramref name="source"/> to an existing instance of <typeparamref name="TDestination"/>.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="destination">The destination.</param>
	void Map(TSource source, TDestination destination);
}