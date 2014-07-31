using System;

namespace Brm
{
	public class PasswdInfo
	{
		//The names come from the passwd structure
		public string Name { get; private set; }

		public UInt32 Id { get; private set; }

		public UInt32 Gid { get; private set; }

		public string Gecos { get; private set; }
		//User info
		public string Dir { get; private set; }

		public string Shell { get; private set; }

		public PasswdInfo (string name, UInt32 id, UInt32 gid,
		                   string gecos, string dir, string shell)
		{
			Name = name;
			Id = id;
			Gid = gid;
			Gecos = gecos;
			Dir = dir;
			Shell = shell;
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
}
