namespace Umbrella.DataAnnotations
{
	public class RequiredNonEmptyCollectionIfTrueAttribute : RequiredNonEmptyCollectionIfAttribute
	{
		public RequiredNonEmptyCollectionIfTrueAttribute(string dependentProperty)
			: base(dependentProperty, Operator.EqualTo, true)
		{
		}
	}
}