namespace Umbrella.DataAnnotations
{
	public class GreaterThanAttribute : IsAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public GreaterThanAttribute(string dependentProperty)
			: base(Operator.GreaterThan, dependentProperty)
		{
		}
	}
}