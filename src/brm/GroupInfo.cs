using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brm
{
	public class GroupInfo
	{
		public string Name { get; private set; }

		public UInt32 Gid { get; private set; }

		public List<string> Members { get; private set; }

		public GroupInfo (string name, UInt32 gid, List<string> members)
		{
			Name = name;
			Gid = gid;
			Members = members;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (Name);
			sb.Append (":x:");
			sb.Append (Gid);
			sb.Append (':');
			for (int i = 0; i < Members.Count; i++) {
				if (i != 0)
					sb.Append (',');
				sb.Append (Members [i]);
			}
			return sb.ToString ();
		}
	}
}