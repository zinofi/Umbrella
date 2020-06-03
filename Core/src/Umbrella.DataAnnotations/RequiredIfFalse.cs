namespace Umbrella.DataAnnotations
{
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