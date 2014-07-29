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
		static void add_group_to (GroupInfo info, string path)
		{
			throw new NotImplementedException ();
		}

		static HashSet<UInt32> get_gids (Dictionary<string, GroupInfo> master_group)
		{
			return new HashSet<UInt32> (master_group.Values.Select (g => g.Gid));
		}

		static Dictionary<UInt32, UInt32> merge_group (string master_group_path, Dictionary<string, GroupInfo> master_group, Dictionary<string, GroupInfo> client_group)
		{
			Dictionary<UInt32, UInt32> toret = new Dictionary<UInt32, UInt32> ();
			HashSet<UInt32> used_master = get_gids (master_group);
			HashSet<UInt32> used_client = get_gids (client_group);

			List<Tuple<UInt32, UInt32>> freelist;



			foreach (KeyValuePair<string, GroupInfo> group in client_group) {
				GroupInfo info = group.Value;
				if (!master_group.ContainsKey (info.Name)) {
					if (!used_master.Contains (info.Gid)) {
						add_group_to (info, master_group_path);
						if (!toret.ContainsKey (info.Gid)) {

						}
					}
				}
			}
			return toret;
		}

		static Dictionary<UInt32, UInt32> merge_passwd (Dictionary<string, PasswdInfo> master_passwd,
		                                                Dictionary<string, PasswdInfo> client_passwd)
		{
			throw new NotImplementedException ();
		}

		static void merge_shadow (Dictionary<string, ShadowInfo> master_shadow, Dictionary<string, ShadowInfo> client_shadow)
		{
			throw new NotImplementedException ();
		}

		static Tables merge_etcfiles (string master, string client)
		{
			Dictionary<string, GroupInfo> master_group = Parser.read_group (master);
			Dictionary<string, GroupInfo> client_group = Parser.read_group (client);

			Dictionary<UInt32, UInt32> gid = merge_group (master + "/group", master_group, client_group);

			Dictionary<string, PasswdInfo> master_passwd = Parser.read_passwd (master);
			Dictionary<string, PasswdInfo> client_passwd = Parser.read_passwd (client);

			Dictionary<UInt32, UInt32> uid = merge_passwd (master_passwd, client_passwd);

			Dictionary<string, ShadowInfo> master_shadow = Parser.read_shadow (master);
			Dictionary<string, ShadowInfo> client_shadow = Parser.read_shadow (client);

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

			if (Filesystem.LookupPath (master) != StatResult.Directory) {
				Console.WriteLine ("Master path doesn't exist or is not a directory");
				return 1;
			}

			if (Filesystem.LookupPath (client) != StatResult.Directory) {
				Console.WriteLine ("Client path doesn't exist or is not a directory");
				return 1;
			}

			if (tree != null && Filesystem.LookupPath (tree) != StatResult.Directory) {
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
