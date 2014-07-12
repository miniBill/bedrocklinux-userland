using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Brm
{
	public static class Parser
	{
		public static Dictionary<string, passwd_info> read_passwd (string path)
		{
			Dictionary<string, passwd_info> users = new Dictionary<string, passwd_info> ();
			using (StreamReader reader = File.OpenText(Path.Combine( path,"passwd"))) {
				for (int line_number = 1; !reader.EndOfStream; line_number++) {
					string line = reader.ReadLine ();
					string[] split = line.Split (':');
					if (split.Length != 7)
						throw new Exception (string.Format ("Invalid record at line{0}: \"{1}\"", line_number, line));
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
							+ line_number);
		
					UInt32 uid = UInt32.Parse (s_uid);
					UInt32 gid = UInt32.Parse (s_gid);

					users.Add (name, new passwd_info (name, uid, gid, gecos, dir, shell));
				}
			}
			return users;
		}

		public static Dictionary<string, GroupInfo> read_group (string path)
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

					List<string> mem = s_mem.Split (':').ToList ();

					groups.Add (name, new GroupInfo (name, gid, mem));
				}
			}

			return groups;
		}

		public static Dictionary<string, shadow_info> read_shadow (string path)
		{
			Dictionary<string, shadow_info> users = new Dictionary<string, shadow_info> ();
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

					users.Add (name, new shadow_info (name, pwd, lstchg, min, max, warn, inact, expire, flag));
				}
			}

			return users;
		}
	}
}