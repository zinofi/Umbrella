namespace Umbrella.DataAnnotations
{
	/// <summary>
	/// Specifies that the value of a property should be required if the value of another named property on the same type is <see langword="false"/>.
	/// </summary>
	/// <seealso cref="RequiredIfAttribute"/>
	public class RequiredIfFalseAttribute : RequiredIfAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredIfFalseAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public RequiredIfFalseAttribute(string dependentProperty)
			: base(dependentProperty, Operator.EqualTo, false)
		{
		}
	}
}