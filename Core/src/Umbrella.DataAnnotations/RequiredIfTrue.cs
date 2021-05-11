namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies that the value of a property should be required if the value of another named property on the same type is <see langword="true"/>.
	/// </summary>
	/// <seealso cref="RequiredIfAttribute"/>
	public class RequiredIfTrueAttribute : RequiredIfAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredIfTrueAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public RequiredIfTrueAttribute(string dependentProperty)
			: base(dependentProperty, Operator.EqualTo, true)
		{
		}
	}
}