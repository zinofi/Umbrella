using System;

namespace Umbrella.FileSystem.Abstractions
{
	/// <summary>
	/// An exception thrown when there has been a problem accessing a file because it could not be found.
	/// </summary>
	/// <seealso cref="Umbrella.FileSystem.Abstractions.UmbrellaFileSystemException" />
	public class UmbrellaFileNotFoundException : UmbrellaFileSystemException
	{
		/// <summary>
		/// Gets the subpath.
		/// </summary>
		public string Subpath { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaFileNotFoundException"/> class.
		/// </summary>
		/// <param name="subpath">The subpath.</param>
		public UmbrellaFileNotFoundException(string subpath)
			: base($"The file located at {subpath} could not be found.")
		{
			Subpath = subpath;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaFileNotFoundException"/> class.
		/// </summary>
		/// <param name="subpath">The subpath.</param>
		/// <param name="innerException">The inner exception.</param>
		public UmbrellaFileNotFoundException(string subpath, Exception innerException)
			: base($"The file located at {subpath} could not be found.", innerException)
		{
			Subpath = subpath;
		}
	}
}