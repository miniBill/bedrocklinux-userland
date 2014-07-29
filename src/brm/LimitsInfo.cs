/*
 * Program.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 */
namespace Brm
{
	class LimitsInfo
	{
		public uint MinGid { get; set; }

		public uint MaxGid { get; set; }

		public uint MinSysGid{ get; set; }

		public uint MaxSysGid{ get; set; }

		public override bool Equals (object obj)
		{
			LimitsInfo other = obj as LimitsInfo;
			if (other == null)
				return false;
			return this == other;
		}

		public static bool operator == (LimitsInfo left, LimitsInfo right)
		{
			return left.MinGid == right.MinGid
				&& left.MaxGid == right.MaxGid
				&& left.MinSysGid == right.MinSysGid
				&& left.MaxSysGid == right.MaxSysGid;
		}

		public static bool operator != (LimitsInfo left, LimitsInfo right)
		{
			return !(left == right);
		}
	}
}
