namespace Umbrella.DataAnnotations
{
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