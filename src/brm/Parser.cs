using System;
using System.Collections.Generic;
using System.IO;
using Brm;
using System.Linq;

public class passwd_info
{
	//The names come from the passwd structure
	public string _name;
	public UInt32 _id;
	public UInt32 _gid;
	public string _gecos;
	//User info
	public string _dir;
	public string _shell;

	public passwd_info (string name, UInt32 id, UInt32 gid,
	                    string gecos, string dir, string shell)
	{
		_name = name;
		_id = id;
		_gid = gid;
		_gecos = gecos;
		_dir = dir;
		_shell = shell;
	}

	public override string ToString ()
	{
		return string.Format ("[passwd_info]");
		/*ostream& operator<<(ostream& stream, passwd_info& info) {
			return stream
				<< info._name << ':'
					<< 'x' << ':'
					<< info._id << ':'
					<< info._gid << ':'
					<< info._gecos << ':'
					<< info._dir << ':'
					<< info._shell;*/
	}
}

public class group_info
{
	//The names come from the group structure
	public string _name;
	public UInt32 _id;
	public List<string> _mem;
	//Members
	public group_info (string name, UInt32 id,
	                   List<string> mem)
	{
		_name = name;
		_id = id;
		_mem = mem;
	}

	public override string ToString ()
	{
		return string.Format ("[group_info]");
		/*
		 * stream
		<< info._name << ':'
		<< 'x' << ':'
		<< info._id << ':';
	for (size_t i = 0; i < info._mem.size(); i++) {
		if (i != 0) {
			stream << ',';
		}
		stream << info._mem[i];
	}
	return stream;*/
	}
}

public class shadow_info
{
	public string _name;
	public string _pwd;
	public long _lstchg;
	public long _min;
	public long _max;
	public long _warn;
	public long _inact;
	public long _expire;
	public ulong _flag;

	public shadow_info (string name, string pwd, long lstchg,
	                    long min, long max, long warn, long inact, long expire,
	                    ulong flag)
	{
		_name = name;
		_pwd = pwd;
		_lstchg = lstchg;
		_min = min;
		_max = max;
		_warn = warn;
		_inact = inact;
		_expire = expire;
		_flag = flag;
	}

	static void print_nonzero (StreamWriter stream, long value)
	{
		stream.Write (':');
		if (value != 0)
			stream.Write (value);
	}

	static void print_nonzero (StreamWriter stream, ulong value)
	{
		stream.Write (':');
		if (value != 0)
			stream.Write (value);
	}

	public override string ToString ()
	{
		return string.Format ("[shadow_info]");
		/*stream
		<< info._name << ':'
		<< info._pwd;
	print_nonzero(stream, info._lstchg);
	stream
		<< ':' << info._min
		<< ':' << info._max;
	print_nonzero(stream, info._warn);
	print_nonzero(stream, info._inact);
	print_nonzero(stream, info._expire);
	print_nonzero(stream, info._flag);
	return stream;*/
	}
};

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

	public static Dictionary<string, group_info> read_group (string path)
	{
		Dictionary<string, group_info> groups = new Dictionary<string, group_info> ();
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

				groups.Add (name, new group_info (name, gid, mem));
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

				//TODO: check for errors here
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