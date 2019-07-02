using System;

namespace Umbrella.DataAccess.Remote.Abstractions
{
	public interface IMultiRemoteItem<TIdentifier, TRemoteSourceType> : IRemoteItem<TIdentifier>
		where TRemoteSourceType : Enum
	{
		TRemoteSourceType Source { get; set; }
	}
}