namespace Umbrella.WebUtilities.Security
{
	/// <summary>
	/// A context object used to track a generated nonce value that is valid for a particular context, e.g. the lifetime of a web request.
	/// This type is registered with DI as a scoped service by default.
	/// </summary>
	public class NonceContext
	{
		/// <summary>
		/// Gets or sets the current nonce value.
		/// </summary>
		public string? Current { get; set; }
	}
}