using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Remote.Abstractions;

namespace Umbrella.DataAccess.Remote
{
	/// <summary>
	/// Serves as the base class for HTTP Services that form part of a group of services that each have the same <typeparamref name="TItem"/> and <typeparamref name="TIdentifier"/> but whose
	/// <typeparamref name="TRemoteSource"/> differ.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
	/// <typeparam name="TRemoteSource">The type of the remote source.</typeparam>
	/// <seealso cref="Umbrella.DataAccess.Remote.GenericHttpService{TItem, TIdentifier}" />
	/// <seealso cref="Umbrella.DataAccess.Remote.Abstractions.IGenericMultiHttpService{TItem, TIdentifier, TRemoteSource}" />
	public abstract class GenericMultiHttpService<TItem, TIdentifier, TRemoteSource> : GenericHttpService<TItem, TIdentifier>, IGenericMultiHttpService<TItem, TIdentifier, TRemoteSource>
		where TItem : class, IMultiRemoteItem<TIdentifier, TRemoteSource>
		where TIdentifier : IEquatable<TIdentifier>
		where TRemoteSource : struct, Enum
	{
		#region Protected Properties		
		/// <summary>
		/// Gets the type of the remote source.
		/// </summary>
		public abstract TRemoteSource RemoteSourceType { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericMultiHttpService{TItem, TIdentifier, TRemoteSource}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="client">The client.</param>
		public GenericMultiHttpService(
			ILogger logger,
			HttpClient client)
			: base(logger, client)
		{
		}
		#endregion

		#region Overridden Methods
		/// <inheritdoc />
		protected override Task AfterItemLoadedAsync(TItem item, CancellationToken cancellationToken)
		{
			item.Source = RemoteSourceType;

			return Task.CompletedTask; ;
		}

		/// <inheritdoc />
		protected override Task AfterItemSavedAsync(TItem item, CancellationToken cancellationToken)
		{
			item.Source = RemoteSourceType;

			return Task.CompletedTask;
		}
		#endregion
	}
}