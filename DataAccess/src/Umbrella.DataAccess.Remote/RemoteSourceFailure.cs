using System;

namespace Umbrella.DataAccess.Remote
{
	public class RemoteSourceFailure<TRemoteSource>
		where TRemoteSource : Enum
	{
		public RemoteSourceFailure(TRemoteSource remoteSourceType, string message)
		{
			RemoteSourceType = remoteSourceType;
			Message = message;
		}

		public TRemoteSource RemoteSourceType { get; set; }
		public string Message { get; set; }
	}
}