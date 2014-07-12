using System;
using System.Collections.Generic;
using System.IO;
using Brm;
using System.Linq;

public class passwd_info
{
	//The names come from the passwd structure
	public string _name;
	public UInt32 _id;
	public UInt32 _gid;
	public string _gecos;
	//User info
	public string _dir;
	public string _shell;

	public passwd_info (string name, UInt32 id, UInt32 gid,
	                    string gecos, string dir, string shell)
	{
		_name = name;
		_id = id;
		_gid = gid;
		_gecos = gecos;
		_dir = dir;
		_shell = shell;
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
