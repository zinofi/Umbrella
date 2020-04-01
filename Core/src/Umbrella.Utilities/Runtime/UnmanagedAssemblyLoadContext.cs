using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Umbrella.Utilities.Runtime
{
	/// <summary>
	/// A custom <see cref="AssemblyLoadContext"/> for loading unmanaged assemblies at runtime.
	/// </summary>
	/// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
	public class UnmanagedAssemblyLoadContext : AssemblyLoadContext
	{
		/// <summary>
		/// Loads the unmanaged library.
		/// </summary>
		/// <param name="absolutePath">The absolute path.</param>
		/// <returns>The <see cref="IntPtr"/> to the assembly.</returns>
		public IntPtr LoadUnmanagedLibrary(string absolutePath) => LoadUnmanagedDll(absolutePath);

		/// <summary>
		/// Loads the unmanaged DLL.
		/// </summary>
		/// <param name="unmanagedDllName">Name of the unmanaged DLL.</param>
		/// <returns>The <see cref="IntPtr"/> to the assembly.</returns>
		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) => LoadUnmanagedDllFromPath(unmanagedDllName);

		/// <summary>
		/// Loads the specified assembly name.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="NotImplementedException">This method is not implemented.</exception>
		protected override Assembly Load(AssemblyName assemblyName) => throw new NotImplementedException();
	}
}