using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Sorting
{
	[Serializable]
    public readonly struct SortExpressionSerializable
    {
		internal SortExpressionSerializable(string memberName, string direction)
		{
			MemberName = memberName;
			Direction = direction;
		}

		public string MemberName { get; }
		public string Direction { get; }
	}
}