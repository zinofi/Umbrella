using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Expressions;
using Xunit;

namespace Umbrella.Utilities.Test.Expressions
{
    public class UmbrellaDynamicExpressionTest
    {
		private class TestType
		{
			public int? NullableIntProperty { get; set; }
			public int IntProperty { get; set; }
			public TypeCode? NullableEnumProperty { get; set; }
			public TypeCode EnumProperty { get; set; }
		}

		//[Fact]
		//public void CreateConstant_NullableIntPropertyMemberAccess()
		//{
		//	var target = Expression.Parameter(typeof(TestType));

		//	var memberAccess = UmbrellaDynamicExpression.CreateMemberAccess(target, nameof(TestType.NullableIntProperty));

		//	//var constant = UmbrellaDynamicExpression.CreateConstant(target, memberAccess!, "1", null);

		//	//var memberAccess1 = UmbrellaDynamicExpression.CreateMemberAccess(target, nameof(TestType.NullableEnumProperty));
		//	//var constant1 = UmbrellaDynamicExpression.CreateConstant(target, memberAccess1!, nameof(TypeCode.Boolean), null);

		//	//int i = 0;
		//}
	}
}