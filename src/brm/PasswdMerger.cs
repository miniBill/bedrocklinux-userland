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

		static void AddUserTo (TextWriter output, PasswdInfo info, string path)
		{
			output.Write (path);
			output.Write (": ");
			output.WriteLine (info);
		}

		public static Dictionary<uint, uint> MergePasswd (TextWriter output,
		                                                  LimitsInfo limits,
		                                                  string masterPasswdPath,
		                                                  Dictionary<string, PasswdInfo> masterPasswds,
		                                                  Dictionary<string, PasswdInfo> clientPasswds)
		{
			var toret = new Dictionary<uint, uint> ();
			var usedMaster = new HashSet<uint> (GetUids (masterPasswds));
			var usedClient = new HashSet<uint> (GetUids (clientPasswds));

			var toAddToMaster = new List<PasswdInfo> ();

			IEnumerator<uint> freelistUser = Utils.InclusiveRange (limits.MinUid, limits.MaxUid)
				.Except (usedMaster.Concat (usedClient)).GetEnumerator ();
			IEnumerator<uint> freelistSystem = Utils.InclusiveRange (limits.MinSysUid, limits.MaxSysUid)
				.Except (usedMaster.Concat (usedClient)).GetEnumerator ();

			if (!freelistUser.MoveNext () || !freelistSystem.MoveNext ())
				throw new AbortException ("Something went wrong with freelists");

			foreach (KeyValuePair<string, PasswdInfo> clientPasswdPair in clientPasswds) {
				PasswdInfo clientUser = clientPasswdPair.Value;
				if (masterPasswds.ContainsKey (clientUser.Name)) {
					PasswdInfo masterPasswd = masterPasswds [clientUser.Name];
					if (clientUser.Id != masterPasswd.Id)
						toret.Add (clientUser.Id, masterPasswd.Id);

					PasswdInfo translated = clientUser.WithId (masterPasswd.Id);

					if (translated != masterPasswd) {
						bool fake = translated == masterPasswd;
						throw new AbortException (string.Format ("User {0} too different between master and client", masterPasswd.Name));
					}
				} else {
					if (usedMaster.Contains (clientUser.Gid)) {
						IEnumerator<uint> source = clientUser.SystemUser ? freelistSystem : freelistUser;
						uint newGid = source.Current;
						if (!source.MoveNext ())
							throw new AbortException ("Ran out of gids");
						toret.Add (clientUser.Gid, newGid);
						toAddToMaster.Add (clientUser.WithId (newGid));
					} else
						toAddToMaster.Add (clientUser);
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

