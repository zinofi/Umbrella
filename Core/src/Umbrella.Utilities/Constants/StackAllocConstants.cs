namespace Umbrella.Utilities.Constants
{
	/// <summary>
	/// Defines constants used to constrain the size of stack allocations when using the <see langword="stackalloc"/> keyword from managed code.
	/// When these limits are passed the allocation will be made on the managed heap instead.
	/// </summary>
	public static class StackAllocConstants
	{
		/// <summary>
		/// The maximum permitted size of a stack allocated char array. As a char has a size of 2 bytes, this equates
		/// to a limit of 512 bytes.
		/// </summary>
		/// <remarks>
		/// See https://vcsjones.dev/stackalloc/
		/// and https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/stackalloc
		/// </remarks>
		public const int MaxCharSize = 256;
	}
}