using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Brm
{
	public static class PasswdMerger
	{
		static IEnumerable<uint> GetUids (Dictionary<string, PasswdInfo> passwd)
		{
			return passwd.Values.Select (g => g.Id);
		}

		public static Dictionary<uint, uint> MergePasswd (TextWriter output,
		                                                  LimitsInfo limits,
		                                                  Dictionary<string, PasswdInfo> masterPasswd,
		                                                  Dictionary<string, PasswdInfo> clientPasswd)
		{
			var toret = new Dictionary<uint, uint> ();
			var usedMaster = new HashSet<uint> (GetUids (masterPasswd));
			var usedClient = new HashSet<uint> (GetUids (clientPasswd));

			var toAddToMaster = new List<PasswdInfo> ();

			IEnumerator<uint> freelistUser = Utils.InclusiveRange (limits.MinUid, limits.MaxUid)
				.Except (usedMaster.Concat (usedClient)).GetEnumerator ();
			IEnumerator<uint> freelistSystem = Utils.InclusiveRange (limits.MinSysUid, limits.MaxSysUid)
				.Except (usedMaster.Concat (usedClient)).GetEnumerator ();

			if (!freelistUser.MoveNext () || !freelistSystem.MoveNext ())
				throw new AbortException ("Something went wrong with freelists");

			foreach (KeyValuePair<string, PasswdInfo> clientGroupPair in clientPasswd) {
				PasswdInfo clientGroup = clientGroupPair.Value;
				if (masterPasswd.ContainsKey (clientGroup.Name)) {
					PasswdInfo masterGroup = masterPasswd [clientGroup.Name];
					if (clientGroup.Id != masterGroup.Id)
						toret.Add (clientGroup.Id, masterGroup.Id);

					PasswdInfo translated = clientGroup.WithId (masterGroup.Id);

					if (translated != masterGroup)
						throw new AbortException (string.Format ("User {0} too different between master and client", masterGroup.Name));
				} else {
					if (usedMaster.Contains (clientGroup.Gid)) {
						IEnumerator<uint> source = clientGroup.SystemUser ? freelistSystem : freelistUser;
						uint newGid = source.Current;
						if (!source.MoveNext ())
							throw new AbortException ("Ran out of gids");
						toret.Add (clientGroup.Gid, newGid);
						toAddToMaster.Add (clientGroup.WithId (newGid));
					} else
						toAddToMaster.Add (clientGroup);
				}
			}

			output.WriteLine ("Users to add:");
			foreach (PasswdInfo adding in toAddToMaster)
				AddUserTo (output, adding, masterPasswdPath);
			output.WriteLine ("Translation table:");
			foreach (KeyValuePair<uint, uint> translation in toret)
				Console.WriteLine (string.Format ("{0} -> {1}", translation.Key, translation.Value));

			return toret;
		}
	}
}

