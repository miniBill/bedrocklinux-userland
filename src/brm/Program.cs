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
 *
 */

using System;
using System.Collections.Generic;

namespace Brm
{
	class MainClass
	{
		static void MergeShadow (Dictionary<string, ShadowInfo> masterShadow,
		                         Dictionary<string, ShadowInfo> clientShadow)
		{
			throw new NotImplementedException (masterShadow.ToString () + clientShadow);
		}

		static Tables MergeEtcfiles (string master, string client)
		{
			LimitsInfo masterLimits = Parser.ReadLimits (master);
			LimitsInfo clientLimits = Parser.ReadLimits (client);

			if (masterLimits != clientLimits)
				throw new AbortException ("UID/GID limits are different between master and client");

			Dictionary<string, GroupInfo> masterGroup = Parser.ReadGroup (masterLimits, master);
			Dictionary<string, GroupInfo> clientGroup = Parser.ReadGroup (masterLimits, client);

			Dictionary<uint, uint> gid = GroupMerger.MergeGroups (Console.Out, masterLimits, master + "/group", masterGroup, clientGroup);

			Dictionary<string, PasswdInfo> masterPasswd = Parser.ReadPasswd (masterLimits, master);
			Dictionary<string, PasswdInfo> clientPasswd = Parser.ReadPasswd (masterLimits, client);

			Dictionary<uint, uint> uid = PasswdMerger.MergePasswd (Console.Out, masterLimits, masterPasswd, clientPasswd);

			Dictionary<string, ShadowInfo> masterShadow = Parser.ReadShadow (master);
			Dictionary<string, ShadowInfo> clientShadow = Parser.ReadShadow (client);

			MergeShadow (masterShadow, clientShadow);

			return new Tables (uid, gid);
		}

		static void WalkTree (string tree, Tables tables)
		{
			throw new NotImplementedException (tree + tables);
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

			try {
				Tables tables = MergeEtcfiles (master, client);

				if (tree != null)
					WalkTree (tree, tables);
			} catch (AbortException e) {
				Console.WriteLine (string.Format ("Huston, we got a problem: {0}", e.Message));
				return 1;
			}

			return 0;
		}
	}
}
