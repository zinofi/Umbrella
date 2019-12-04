namespace Umbrella.Utilities.Data.Abstractions
{
	/// <summary>
	/// An abstraction representing an expression descriptor.
	/// </summary>
	public interface IExpressionDescriptor
	{
		/// <summary>
		/// Gets or sets the name of the member.
		/// </summary>
		public string MemberName { get; set; }
	}
}