using System;
using System.Runtime.InteropServices;

namespace Umbrella.Utilities.Sorting
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
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