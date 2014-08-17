/*
 * Parser.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Brm
{
	public static class Parser
	{
		const string LimitParseError = "Invalid adduser.conf at line {1} ({0}): '{2}'";

		static uint ParseValue (int lineNumber, string line)
		{
			int equal = line.IndexOf ('=');
			if (equal < 0)
				throw new AbortException (
					string.Format (LimitParseError, "missing '='", lineNumber, line));
			string right = line.Substring (equal + 1);
			right = right.Trim ();
			if (right.Length == 0)
				throw new AbortException (
					string.Format (LimitParseError, "missing value", lineNumber, line));
			uint value;
			if (!uint.TryParse (right, out value))
				throw new AbortException (
					string.Format (LimitParseError, "missing value", lineNumber, line));
			return value;
		}

		public static LimitsInfo ReadLimits (string path)
		{
			uint? minSysUid = null;
			uint? maxSysUid = null;
			uint? minUid = null;
			uint? maxUid = null;
			uint? minSysGid = null;
			uint? maxSysGid = null;
			uint? minGid = null;
			uint? maxGid = null;
			using (StreamReader reader = File.OpenText (Path.Combine (path, "adduser.conf"))) {
				for (int lineNumber = 1; !reader.EndOfStream; lineNumber++) {
					string line = reader.ReadLine ();
					if (line.Length == 0 || line [0] == '#')
						continue;
					if (line.StartsWith ("FIRST_SYSTEM_UID", StringComparison.Ordinal))
						minSysUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_SYSTEM_UID", StringComparison.Ordinal))
						maxSysUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("FIRST_UID", StringComparison.Ordinal))
						minUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_UID", StringComparison.Ordinal))
						maxUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("FIRST_SYSTEM_GID", StringComparison.Ordinal))
						minSysGid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_SYSTEM_GID", StringComparison.Ordinal))
						maxSysGid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("FIRST_GID", StringComparison.Ordinal))
						minGid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_GID", StringComparison.Ordinal))
						maxGid = ParseValue (lineNumber, line);
				}
			}

			if (!minSysUid.HasValue)
				throw new AbortException ("Missing FIRST_SYSTEM_UID from adduser.conf");
			if (!maxSysUid.HasValue)
				throw new AbortException ("Missing LAST_SYSTEM_UID from adduser.conf");
			if (!minSysGid.HasValue)
				throw new AbortException ("Missing FIRST_SYSTEM_GID from adduser.conf");
			if (!maxSysGid.HasValue)
				throw new AbortException ("Missing LAST_SYSTEM_GID from adduser.conf");
			if (!minUid.HasValue)
				throw new AbortException ("Missing FIRST_UID from adduser.conf");
			if (!maxUid.HasValue)
				throw new AbortException ("Missing LAST_UID from adduser.conf");
			if (!minGid.HasValue)
				throw new AbortException ("Missing FIRST_GID from adduser.conf");
			if (!maxGid.HasValue)
				throw new AbortException ("Missing LAST_GID from adduser.conf");

			return new LimitsInfo (
				minSysUid.Value, maxSysUid.Value, minSysGid.Value, maxSysGid.Value, 
				minUid.Value, maxUid.Value, minGid.Value, maxGid.Value);
		}

		public static Dictionary<string, PasswdInfo> ReadPasswd (LimitsInfo limits, string path)
		{
			var users = new Dictionary<string, PasswdInfo> ();
			using (StreamReader reader = File.OpenText (Path.Combine (path, "passwd"))) {
				for (int lineNumber = 1; !reader.EndOfStream; lineNumber++) {
					string line = reader.ReadLine ();
					string[] split = line.Split (':');
					if (split.Length != 7)
						throw new AbortException (string.Format ("Invalid record at line{0}: \"{1}\"", lineNumber, line));
					string name = split [0];
					string shadow = split [1];
					string s_uid = split [2];
					string s_gid = split [3];
					string gecos = split [4];
					string dir = split [5];
					string shell = split [6];

					if (shadow != "x")
						throw new AbortException ("Unexpected \"" + shadow
						+ "\" instead of \"x\" as second field on line "
						+ lineNumber);
		
					UInt32 uid = UInt32.Parse (s_uid);
					UInt32 gid = UInt32.Parse (s_gid);

					bool sys = uid < limits.MinUid || uid > limits.MaxUid;

					users.Add (name, new PasswdInfo (name, uid, gid, gecos, dir, shell, sys));
				}
			}
			return users;
		}

		static char[] semicolon = { ':' };

		public static Dictionary<string, GroupInfo> ReadGroup (LimitsInfo limits, string path)
		{
			var groups = new Dictionary<string, GroupInfo> ();
			using (StreamReader reader = File.OpenText (Path.Combine (path, "group"))) {
				for (int line_number = 1; !reader.EndOfStream; line_number++) {
					string line = reader.ReadLine ();
					string[] split = line.Split (':');
				
					if (split.Length != 4)
						throw new AbortException (string.Format ("Invalid record at line{0}: \"{1}\"", line_number, line));
					string name = split [0];
					string shadow = split [1];
					string s_gid = split [2];
					string s_mem = split [3];

					if (shadow != "x")
						throw new AbortException ("Unexpected \"" + shadow
						+ "\" instead of \"x\" as second field on line "
						+ line_number);

					UInt32 gid = UInt32.Parse (s_gid);

					bool sys = gid < limits.MinGid || gid > limits.MaxGid;

					List<string> mem = s_mem.Split (semicolon, StringSplitOptions.RemoveEmptyEntries).ToList ();

					groups.Add (name, new GroupInfo (name, gid, mem, sys));
				}
			}

			return groups;
		}

		public static Dictionary<string, ShadowInfo> ReadShadow (string path)
		{
			var users = new Dictionary<string, ShadowInfo> ();
			using (StreamReader reader = File.OpenText (Path.Combine (path, "shadow")))
				for (int line_number = 1; !reader.EndOfStream; line_number++) {
					string line = reader.ReadLine ();
					string[] split = line.Split (':');
					if (split.Length != 9)
						throw new AbortException (string.Format ("Invalid record at line{0}: \"{1}\"", line_number, line));

					string name = split [0];
					string pwd = split [1];
					string s_lstchg = split [2];
					string s_min = split [3];
					string s_max = split [4];
					string s_warn = split [5];
					string s_inact = split [6];
					string s_expire = split [7];
					string s_flag = split [8];

					long lstchg = long.Parse (s_lstchg);
					long min = long.Parse (s_min);
					long max = long.Parse (s_max);
					long warn = long.Parse (s_warn);
					long inact = long.Parse (s_inact);
					long expire = long.Parse (s_expire);
					ulong flag = ulong.Parse (s_flag);

					users.Add (name, new ShadowInfo (name, pwd, lstchg, min, max, warn, inact, expire, flag));
				}

			return users;
		}
	}
}
