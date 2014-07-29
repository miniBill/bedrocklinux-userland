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
using System.Linq;

namespace Brm
{
	class MainClass
	{
		static void AddGroupTo (GroupInfo info, string path)
		{
			throw new NotImplementedException ();
		}

		static void MergeGroupInto (GroupInfo merging, string masterGroupsPath)
		{
			throw new NotImplementedException ();
		}

		static IEnumerable<uint> GetGids (Dictionary<string, GroupInfo> masterGroup)
		{
			return masterGroup.Values.Select (g => g.Gid);
		}

		static Dictionary<uint, uint> MergeGroups (LimitsInfo limits,
		                                           string masterGroupsPath,
		                                           Dictionary<string, GroupInfo> masterGroups,
		                                           Dictionary<string, GroupInfo> clientGroups)
		{
			Dictionary<uint, uint> toret = new Dictionary<uint, uint> ();
			HashSet<uint> usedMaster = new HashSet<uint> (GetGids (masterGroups));
			HashSet<uint> usedClient = new HashSet<uint> (GetGids (clientGroups));

			List<GroupInfo> toAddToMaster = new List<GroupInfo> ();
			List<GroupInfo> toMergeIntoMaster = new List<GroupInfo> ();

			IEnumerator<uint> freelistUser = Utils.InclusiveRange (limits.MinGid, limits.MaxGid)
				.Except (usedMaster.Concat (usedClient)).GetEnumerator ();
			IEnumerator<uint> freelistSystem = Utils.InclusiveRange (limits.MinSysGid, limits.MaxSysGid)
				.Except (usedMaster.Concat (usedClient)).GetEnumerator ();

			if (!freelistUser.MoveNext () || !freelistSystem.MoveNext ())
				throw new AbortException ("Something went wrong with freelists");

			foreach (KeyValuePair<string, GroupInfo> clientGroupPair in clientGroups) {
				GroupInfo clientGroup = clientGroupPair.Value;
				if (masterGroups.ContainsKey (clientGroup.Name)) {
					GroupInfo masterGroup = masterGroups [clientGroup.Name];
					if (clientGroup.Gid != masterGroup.Gid)
						toret.Add (clientGroup.Gid, masterGroup.Gid);
					toMergeIntoMaster.Add (clientGroup.WithGid (masterGroup.Gid));
				} else {
					if (usedMaster.Contains (clientGroup.Gid)) {
						IEnumerator<uint> source = clientGroup.SystemGroup ? freelistSystem : freelistUser;
						uint newGid = source.Current;
						if (!source.MoveNext ())
							throw new AbortException ("Ran out of gids");
						toret.Add (clientGroup.Gid, newGid);
						toAddToMaster.Add (clientGroup.WithGid (newGid));
					} else
						toAddToMaster.Add (clientGroup);
				}
			}

			foreach (GroupInfo adding in toAddToMaster)
				AddGroupTo (adding, masterGroupsPath);
			foreach (GroupInfo merging in toMergeIntoMaster)
				MergeGroupInto (merging, masterGroupsPath);

			return toret;
		}

		static Dictionary<uint, uint> MergePasswd (Dictionary<string, PasswdInfo> masterPasswd,
		                                           Dictionary<string, PasswdInfo> clientPasswd)
		{
			throw new NotImplementedException ();
		}

		static void MergeShadow (Dictionary<string, ShadowInfo> masterShadow,
		                         Dictionary<string, ShadowInfo> clientShadow)
		{
			throw new NotImplementedException ();
		}

		static Tables MergeEtcfiles (string master, string client)
		{
			LimitsInfo masterLimits = Parser.ReadLimits (master);
			LimitsInfo clientLimits = Parser.ReadLimits (client);

			if (masterLimits != clientLimits)
				throw new AbortException ("UID/GID limits are different between master and client");

			Dictionary<string, GroupInfo> masterGroup = Parser.ReadGroup (masterLimits, master);
			Dictionary<string, GroupInfo> clientGroup = Parser.ReadGroup (masterLimits, client);

			Dictionary<uint, uint> gid = MergeGroups (masterLimits, master + "/group", masterGroup, clientGroup);

			Dictionary<string, PasswdInfo> masterPasswd = Parser.ReadPasswd (master);
			Dictionary<string, PasswdInfo> clientPasswd = Parser.ReadPasswd (client);

			Dictionary<uint, uint> uid = MergePasswd (masterPasswd, clientPasswd);

			Dictionary<string, ShadowInfo> masterShadow = Parser.ReadShadow (master);
			Dictionary<string, ShadowInfo> clientShadow = Parser.ReadShadow (client);

			MergeShadow (masterShadow, clientShadow);

			return new Tables (uid, gid);
		}

		static void WalTree (string tree, Tables tables)
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

			Tables tables = MergeEtcfiles (master, client);

			if (tree != null) {
				WalTree (tree, tables);
			}

			return -1;
		}
	}
}
