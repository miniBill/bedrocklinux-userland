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
using System.IO;

namespace Brm
{
	class MainClass
	{
		static void AddGroupTo (TextWriter output, GroupInfo info, string path)
		{
			output.Write (path);
			output.Write (": ");
			output.WriteLine (info);
		}

		static void MergeGroupInto (TextWriter output, GroupInfo info, string path)
		{
			output.Write (path);
			output.Write (": ");
			output.WriteLine (info);
		}

		static IEnumerable<uint> GetGids (Dictionary<string, GroupInfo> masterGroup)
		{
			return masterGroup.Values.Select (g => g.Gid);
		}

		static Dictionary<uint, uint> MergeGroups (TextWriter output,
		                                           LimitsInfo limits,
		                                           string masterGroupsPath,
		                                           Dictionary<string, GroupInfo> masterGroups,
		                                           Dictionary<string, GroupInfo> clientGroups)
		{
			var toret = new Dictionary<uint, uint> ();
			var usedMaster = new HashSet<uint> (GetGids (masterGroups));
			var usedClient = new HashSet<uint> (GetGids (clientGroups));

			var toAddToMaster = new List<GroupInfo> ();
			var toMergeIntoMaster = new List<GroupInfo> ();

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
					if (!clientGroup.Members.OrderBy (t => t).Distinct ().SequenceEqual (masterGroup.Members.OrderBy (t => t).Distinct ()))
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

			output.WriteLine ("Groups to add:");
			foreach (GroupInfo adding in toAddToMaster)
				AddGroupTo (output, adding, masterGroupsPath);
			output.WriteLine ("Groups to merge:");
			foreach (GroupInfo merging in toMergeIntoMaster)
				MergeGroupInto (output, merging, masterGroupsPath);

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

			Dictionary<uint, uint> gid = MergeGroups (Console.Out, masterLimits, master + "/group", masterGroup, clientGroup);

			Dictionary<string, PasswdInfo> masterPasswd = Parser.ReadPasswd (master);
			Dictionary<string, PasswdInfo> clientPasswd = Parser.ReadPasswd (client);

			Dictionary<uint, uint> uid = MergePasswd (masterPasswd, clientPasswd);

			Dictionary<string, ShadowInfo> masterShadow = Parser.ReadShadow (master);
			Dictionary<string, ShadowInfo> clientShadow = Parser.ReadShadow (client);

			MergeShadow (masterShadow, clientShadow);

			return new Tables (uid, gid);
		}

		static void WalkTree (string tree, Tables tables)
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
