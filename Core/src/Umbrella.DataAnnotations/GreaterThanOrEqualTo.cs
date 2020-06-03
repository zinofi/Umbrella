namespace Umbrella.DataAnnotations
{
	public class GreaterThanOrEqualToAttribute : IsAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GreaterThanOrEqualToAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public GreaterThanOrEqualToAttribute(string dependentProperty)
			: base(Operator.GreaterThanOrEqualTo, dependentProperty)
		{
		}
	}
}