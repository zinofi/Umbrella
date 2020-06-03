namespace Umbrella.DataAnnotations
{
	public class LessThanAttribute : IsAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LessThanAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public LessThanAttribute(string dependentProperty)
			: base(Operator.LessThan, dependentProperty)
		{
		}
	}
}