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
using Brm;
using System.Linq;

namespace Brm
{
	class MainClass
	{
		static void add_group_to (group_info info, string path)
		{
			throw new NotImplementedException ();
		}

		static HashSet<UInt32> get_gids (Dictionary<string, group_info> master_group)
		{
			return new HashSet<UInt32> (master_group.Values.Select (g => g._id));
		}

		static Dictionary<UInt32, UInt32> merge_group (string master_group_path, Dictionary<string, group_info> master_group, Dictionary<string, group_info> client_group)
		{
			Dictionary<UInt32, UInt32> toret = new Dictionary<UInt32, UInt32> ();
			HashSet<UInt32> used_master = get_gids (master_group);
			HashSet<UInt32> used_client = get_gids (client_group);

			List<Tuple<UInt32, UInt32>> freelist;

			foreach (var group in client_group) {
				group_info info = group.Value;
				if (!master_group.ContainsKey (info._name)) {
					if (!used_master.Contains (info._id)) {
						add_group_to (info, master_group_path);
						if (!toret.ContainsKey (info._id)) {

						}
					}
				}
			}
			return toret;
		}

		static Dictionary<UInt32, UInt32> merge_passwd (Dictionary<string, passwd_info> master_passwd,
		                                                Dictionary<string, passwd_info> client_passwd)
		{
			throw new NotImplementedException ();
		}

		static void merge_shadow (Dictionary<string, shadow_info> master_shadow, Dictionary<string, shadow_info> client_shadow)
		{
			throw new NotImplementedException ();
		}

		static Tables merge_etcfiles (string master, string client)
		{
			Dictionary<string, group_info> master_group = Parser.read_group (master);
			Dictionary<string, group_info> client_group = Parser.read_group (client);

			Dictionary<UInt32, UInt32> gid = merge_group (master + "/group", master_group, client_group);

			Dictionary<string, passwd_info> master_passwd = Parser.read_passwd (master);
			Dictionary<string, passwd_info> client_passwd = Parser.read_passwd (client);

			Dictionary<UInt32, UInt32> uid = merge_passwd (master_passwd, client_passwd);

			Dictionary<string, shadow_info> master_shadow = Parser.read_shadow (master);
			Dictionary<string, shadow_info> client_shadow = Parser.read_shadow (client);

			merge_shadow (master_shadow, client_shadow);

			return new Tables (uid, gid);
		}

		static void walk_tree (string tree, Tables tables)
		{
			throw new NotImplementedException ();
		}

		public static int Main (string[] argv)
		{
			if (argv.Length < 2) {
				Console.WriteLine ("Usage: brm <master etc> <client etc> [<client path>]");
				Console.WriteLine ("The optional parameter, if supplied, enables owner fixing.");
				return 1;
			}

			string master = argv [0];
			string client = argv [1];
			string tree = argv.Length >= 3 ? argv [2] : null;

			if (Filesystem.lookup_path (master) != stat_result.Directory) {
				Console.WriteLine ("Master path doesn't exist or is not a directory");
				return 1;
			}

			if (Filesystem.lookup_path (client) != stat_result.Directory) {
				Console.WriteLine ("Client path doesn't exist or is not a directory");
				return 1;
			}

			if (tree != null && Filesystem.lookup_path (tree) != stat_result.Directory) {
				Console.WriteLine ("Tree path doesn't exist or is not a directory");
				return 1;
			}

			Tables tables = merge_etcfiles (master, client);

			if (tree != null) {
				walk_tree (tree, tables);
			}

			return -1;
		}
	}
}
class Tables
{
	public Dictionary<UInt32, UInt32> Uid{ get; private set; }

	public Dictionary<UInt32, UInt32> Gid{ get; private set; }

	public UInt32 translate (UInt32 uid)
	{
		throw new NotImplementedException ();
	}

	public Tables (Dictionary<UInt32, UInt32> uid,
	               Dictionary<UInt32, UInt32> gid)
	{
		Uid = uid;
		Gid = gid;
	}
}