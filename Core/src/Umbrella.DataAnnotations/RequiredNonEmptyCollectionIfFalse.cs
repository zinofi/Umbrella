namespace Umbrella.DataAnnotations
{
	public class RequiredNonEmptyCollectionIfFalseAttribute : RequiredNonEmptyCollectionIfAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredNonEmptyCollectionIfFalseAttribute"/> class.
		/// </summary>
		/// <param name="dependentProperty">The dependent property.</param>
		public RequiredNonEmptyCollectionIfFalseAttribute(string dependentProperty)
			: base(dependentProperty, Operator.EqualTo, false)
		{
		}
	}
}