using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAnnotations
{
	public class RequiredNonEmptyCollectionIfFalseAttribute : RequiredNonEmptyCollectionIfAttribute
	{
		public RequiredNonEmptyCollectionIfFalseAttribute(string dependentProperty)
			: base(dependentProperty, Operator.EqualTo, false)
		{
		}
	}
}