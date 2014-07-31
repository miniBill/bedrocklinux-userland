/*
 * Program.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 * This program will allow merging etcfiles, and walking
 * of a filesystem to fix file owners.
 */

using System;
using System.Collections.Generic;

namespace Brm
{
	class Tables
	{
		public Dictionary<UInt32, UInt32> Uid { get; private set; }

		public Dictionary<UInt32, UInt32> Gid { get; private set; }

		public UInt32 TranslateUid (UInt32 uid)
		{
			return Uid.ContainsKey (uid) ? Uid [uid] : uid;
		}

		public UInt32 TranslateGid (UInt32 gid)
		{
			return Gid.ContainsKey (gid) ? Gid [gid] : gid;
		}

		public Tables (Dictionary<UInt32, UInt32> uid,
		               Dictionary<UInt32, UInt32> gid)
		{
			Uid = uid;
			Gid = gid;
		}
	}
}