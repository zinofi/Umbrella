using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.StringMetric
{
	public enum StringMetricAlgorithmType
	{
		DamerauLevenshtein
	}

	public static class StringMetricAlgorithms
	{
		public static int GetEditDistanceDamerauLevenshtein(string first, string second)
		{
			int len_orig = first.Length;
			int len_diff = second.Length;

			var matrix = new int[len_orig + 1, len_diff + 1];
			for (int i = 0; i <= len_orig; i++)
				matrix[i, 0] = i;
			for (int j = 0; j <= len_diff; j++)
				matrix[0, j] = j;

			for (int i = 1; i <= len_orig; i++)
			{
				for (int j = 1; j <= len_diff; j++)
				{
					int cost = second[j - 1] == first[i - 1] ? 0 : 1;
					var vals = new int[] {
				matrix[i - 1, j] + 1,
				matrix[i, j - 1] + 1,
				matrix[i - 1, j - 1] + cost
			};
					matrix[i, j] = vals.Min();
					if (i > 1 && j > 1 && first[i - 1] == second[j - 2] && first[i - 2] == second[j - 1])
						matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
				}
			}
			return matrix[len_orig, len_diff];
		}
	}
}