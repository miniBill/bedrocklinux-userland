/*
 * GroupMerger.cs
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
	public static class GroupMerger
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
			return masterGroup.Values.Select (g => g.Id);
		}

		public static Dictionary<uint, uint> MergeGroups (TextWriter output,
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
					if (clientGroup.Id != masterGroup.Id)
						toret.Add (clientGroup.Id, masterGroup.Id);
					if (!masterGroup.Members.Contains (clientGroup.Members))
						toMergeIntoMaster.Add (clientGroup.WithId (masterGroup.Id));
				} else {
					if (usedMaster.Contains (clientGroup.Id)) {
						IEnumerator<uint> source = clientGroup.SystemGroup ? freelistSystem : freelistUser;
						uint newGid = source.Current;
						if (!source.MoveNext ())
							throw new AbortException ("Ran out of gids");
						toret.Add (clientGroup.Id, newGid);
						toAddToMaster.Add (clientGroup.WithId (newGid));
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
			output.WriteLine ("Translation table:");
			foreach (KeyValuePair<uint, uint> translation in toret)
				Console.WriteLine (string.Format ("{0} -> {1}", translation.Key, translation.Value));

			return toret;
		}
	}
}

