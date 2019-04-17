using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.FileSystem.Abstractions
{
	/// <summary>
	/// Constants used with the Umbrella file system.
	/// </summary>
    public class UmbrellaFileSystemConstants
    {
		/// <summary>
		/// A constant representing a kilo-byte (Non-SI version).
		/// </summary>
		public const long KB = 1024;

		/// <summary>
		/// A constant representing a megabyte (Non-SI version).
		/// </summary>
		public const long MB = 1024 * KB;

		/// <summary>
		/// A constant representing a gigabyte (Non-SI version).
		/// </summary>
		public const long GB = 1024 * MB;

		/// <summary>
		/// Small buffer size better suited for use with the UmbrellaDiskFileProvider and UmbrellaDiskFileInfo.
		/// </summary>
		public const int SmallBufferSize = (int)(4 * KB);

		/// <summary>
		/// Large buffer size better suited for use with the UmbrellaAzureBlobStorageFileProvider and UmbrellaAzureBlobStorageFileInfo.
		/// </summary>
		public const int LargeBufferSize = (int)(4 * MB);
	}
}