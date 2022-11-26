using System;
using Umbrella.Utilities.Data.Pagination;

namespace Umbrella.DataAccess.Remote;

/// <summary>
/// A <see cref="PaginatedResultModel{TItem}"/> implementation that doesn't do anything and whose items are of
/// type <see cref="NoopRemoteItem{TIdentifier}" />.
/// </summary>
/// <typeparam name="TIdentifier">The type of the item identifier.</typeparam>
/// <seealso cref="PaginatedResultModel{TItem}" />
public class NoopPaginatedResultModel<TIdentifier> : PaginatedResultModel<NoopRemoteItem<TIdentifier>>
	where TIdentifier : IEquatable<TIdentifier>
{
}