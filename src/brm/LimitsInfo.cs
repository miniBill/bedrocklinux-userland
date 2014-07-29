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
	public class LimitsInfo
	{
		public LimitsInfo (uint minSysUid, uint maxSysUid, uint minSysGid, uint maxSysGid,
		                   uint minUid, uint maxUid, uint minGid, uint maxGid)
		{
			MinSysUid = minSysUid;
			MaxSysUid = maxSysUid;
			MinSysGid = minSysGid;
			MaxSysGid = maxSysGid;
			
			MinUid = minUid;
			MaxUid = maxUid;
			MinGid = minGid;
			MaxGid = maxGid;
		}

		public uint MinUid { get; private set; }

		public uint MaxUid { get; private set; }

		public uint MinSysUid { get; private set; }

		public uint MaxSysUid { get; private set; }

		public uint MinGid { get; private set; }

		public uint MaxGid { get; private set; }

		public uint MinSysGid { get; private set; }

		public uint MaxSysGid { get; private set; }

		public override bool Equals (object obj)
		{
			LimitsInfo other = obj as LimitsInfo;
			if (other == null)
				return false;
			return this == other;
		}

		public override int GetHashCode ()
		{
			return (int)(MinGid
				+ 37 * (MaxGid
				+ 37 * (MinSysGid
				+ 37 * (MaxSysGid
				+ 37 * (MinUid
				+ 37 * (MaxUid
				+ 37 * (MinSysUid
				+ 37 * (MaxSysUid))))))));
		}

		public static bool operator == (LimitsInfo left, LimitsInfo right)
		{
			return left.MinGid == right.MinGid
				&& left.MaxGid == right.MaxGid
				&& left.MinSysGid == right.MinSysGid
				&& left.MaxSysGid == right.MaxSysGid
				&& left.MinUid == right.MinUid
				&& left.MaxUid == right.MaxUid
				&& left.MinSysUid == right.MinSysUid
				&& left.MaxSysUid == right.MaxSysUid;
		}

		public static bool operator != (LimitsInfo left, LimitsInfo right)
		{
			return !(left == right);
		}
	}
}
