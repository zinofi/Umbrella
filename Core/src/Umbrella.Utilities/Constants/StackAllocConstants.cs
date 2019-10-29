namespace Umbrella.Utilities.Constants
{
	/// <summary>
	/// Defines constants used to constrain the size of stack allocations when using the <see langword="stackalloc"/> keyword from managed code.
	/// When these limits are passed the allocation will be made on the managed heap instead.
	/// </summary>
	public static class StackAllocConstants
	{
		/// <summary>
		/// The maximum permitted size of a stack allocated char array.
		/// </summary>
		public const int MaxCharSize = 128;
	}
}