namespace Umbrella.DataAnnotations
{
	public class EqualToAttribute : IsAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EqualToAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public EqualToAttribute(string dependentProperty)
			: base(Operator.EqualTo, dependentProperty)
		{
		}
	}
}