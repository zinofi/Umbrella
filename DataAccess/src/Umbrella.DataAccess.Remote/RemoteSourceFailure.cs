using System;

namespace Umbrella.DataAccess.Remote
{
	/// <summary>
	/// Reprents a failure that has taken place when interacting with data stored remotely.
	/// </summary>
	/// <typeparam name="TRemoteSource">The type of the remote source.</typeparam>
	public class RemoteSourceFailure<TRemoteSource>
		where TRemoteSource : Enum
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RemoteSourceFailure{TRemoteSource}"/> class.
		/// </summary>
		/// <param name="remoteSourceType">Type of the remote source.</param>
		/// <param name="message">The message.</param>
		public RemoteSourceFailure(TRemoteSource remoteSourceType, string message)
		{
			RemoteSourceType = remoteSourceType;
			Message = message;
		}

		/// <summary>
		/// Gets or sets the type of the remote source.
		/// </summary>
		public TRemoteSource RemoteSourceType { get; set; }

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		public string Message { get; set; }
	}
}