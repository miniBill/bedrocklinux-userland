using System.Collections.Generic;

namespace Brm
{
	public static class Utils
	{
		public static IEnumerable<uint> InclusiveRange (uint min, uint max)
		{
			while (min <= max)
				yield return min++;
		}
	}
}
