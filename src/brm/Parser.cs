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
			using (StreamReader reader=File.OpenText(Path.Combine(path,"adduser.conf"))) {
				for (int lineNumber=1; !reader.EndOfStream; lineNumber++) {
					string line = reader.ReadLine ();
					if (line.Length == 0 || line [0] == '#')
						continue;
					if (line.StartsWith ("FIRST_SYSTEM_UID"))
						minSysUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_SYSTEM_UID"))
						maxSysUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("FIRST_UID"))
						minUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_UID"))
						maxUid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("FIRST_SYSTEM_GID"))
						minSysGid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_SYSTEM_GID"))
						maxSysGid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("FIRST_GID"))
						minGid = ParseValue (lineNumber, line);
					else if (line.StartsWith ("LAST_GID"))
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

		public static Dictionary<string, PasswdInfo> ReadPasswd (string path)
		{
			Dictionary<string, PasswdInfo> users = new Dictionary<string, PasswdInfo> ();
			using (StreamReader reader = File.OpenText(Path.Combine( path,"passwd"))) {
				for (int lineNumber = 1; !reader.EndOfStream; lineNumber++) {
					string line = reader.ReadLine ();
					string[] split = line.Split (':');
					if (split.Length != 7)
						throw new Exception (string.Format ("Invalid record at line{0}: \"{1}\"", lineNumber, line));
					string name = split [0];
					string shadow = split [1];
					string s_uid = split [2];
					string s_gid = split [3];
					string gecos = split [4];
					string dir = split [5];
					string shell = split [6];

					if (shadow != "x") 
						throw new Exception ("Unexpected \"" + shadow
							+ "\" instead of \"x\" as second field on line "
							+ lineNumber);
		
					UInt32 uid = UInt32.Parse (s_uid);
					UInt32 gid = UInt32.Parse (s_gid);

					users.Add (name, new PasswdInfo (name, uid, gid, gecos, dir, shell));
				}
			}
			return users;
		}

		public static Dictionary<string, GroupInfo> ReadGroup (LimitsInfo limits, string path)
		{
			Dictionary<string, GroupInfo> groups = new Dictionary<string, GroupInfo> ();
			using (StreamReader reader = File.OpenText(Path.Combine( path,"group"))) {
				for (int line_number = 1; !reader.EndOfStream; line_number++) {
					string line = reader.ReadLine ();
					string[] split = line.Split (':');
				
					if (split.Length != 4)
						throw new Exception (string.Format ("Invalid record at line{0}: \"{1}\"", line_number, line));
					string name = split [0];
					string shadow = split [1];
					string s_gid = split [2];
					string s_mem = split [3];

					if (shadow != "x") 
						throw new Exception ("Unexpected \"" + shadow
							+ "\" instead of \"x\" as second field on line "
							+ line_number);

					UInt32 gid = UInt32.Parse (s_gid);

					bool sys = gid < limits.MinGid || gid > limits.MaxGid;

					List<string> mem = s_mem.Split (':').ToList ();

					groups.Add (name, new GroupInfo (name, gid, mem, sys));
				}
			}

			return groups;
		}

		public static Dictionary<string, ShadowInfo> ReadShadow (string path)
		{
			Dictionary<string, ShadowInfo> users = new Dictionary<string, ShadowInfo> ();
			using (StreamReader reader = File.OpenText(Path.Combine( path,"shadow"))) {
				for (int line_number = 1; !reader.EndOfStream; line_number++) {
					string line = reader.ReadLine ();
					string[] split = line.Split (':');
					if (split.Length != 9)
						throw new Exception (string.Format ("Invalid record at line{0}: \"{1}\"", line_number, line));

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
			}

			return users;
		}
	}
}