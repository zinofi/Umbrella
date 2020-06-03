namespace Umbrella.DataAnnotations
{
	public class NotEqualToAttribute : IsAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotEqualToAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public NotEqualToAttribute(string dependentProperty)
			: base(Operator.NotEqualTo, dependentProperty)
		{
		}
	}
}