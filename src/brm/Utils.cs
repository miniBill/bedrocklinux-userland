/*
 * Utils.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 */


using System.Collections.Generic;
using System.Linq;

namespace Brm
{
	public static class Utils
	{
		public static IEnumerable<uint> InclusiveRange (uint min, uint max)
		{
			while (min <= max)
				yield return min++;
		}

		public static bool Contains<T> (this IReadOnlyCollection<T> haystack, IEnumerable<T> needles)
		{
			return needles.All (haystack.Contains);
		}
	}
}
