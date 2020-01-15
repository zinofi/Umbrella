using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Umbrella.Utilities.Runtime
{
	// TODO: Maybe move to its own package if we hit problems.
	public class UnmanagedAssemblyLoadContext : AssemblyLoadContext
	{
		public IntPtr LoadUnmanagedLibrary(string absolutePath) => LoadUnmanagedDll(absolutePath);
		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) => LoadUnmanagedDllFromPath(unmanagedDllName);
		protected override Assembly Load(AssemblyName assemblyName) => throw new NotImplementedException();
	}
}