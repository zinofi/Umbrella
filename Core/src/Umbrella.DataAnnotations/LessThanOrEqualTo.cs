namespace Umbrella.DataAnnotations
{
	public class LessThanOrEqualToAttribute : IsAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LessThanOrEqualToAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public LessThanOrEqualToAttribute(string dependentProperty)
			: base(Operator.LessThanOrEqualTo, dependentProperty)
		{
		}
	}
}