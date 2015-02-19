/*
 * PasswdInfo.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 */


using System;

namespace Brm
{
	public class PasswdInfo
	{
		//The names come from the passwd structure
		public string Name { get; private set; }

		public UInt32 Id { get; private set; }

		public UInt32 Gid { get; private set; }

		//User info
		public string Gecos { get; private set; }

		public string Dir { get; private set; }

		public string Shell { get; private set; }

		public bool SystemUser { get; private set; }

		public PasswdInfo (string name, UInt32 id, UInt32 gid,
		                   string gecos, string dir, string shell,
		                   bool systemUser)
		{
			Name = name;
			Id = id;
			Gid = gid;
			Gecos = gecos;
			Dir = dir;
			Shell = shell;
			SystemUser = systemUser;
		}

		public PasswdInfo WithId (uint newId)
		{
			return new PasswdInfo (Name, newId, Gid, Gecos, Dir, Shell, SystemUser);
		}

		public override bool Equals (object obj)
		{
			var other = obj as PasswdInfo;
			return other != null && this == other;
		}

		public static bool operator == (PasswdInfo left, PasswdInfo right)
		{
			return left.Name == right.Name && left.Id == right.Id && left.Gid == right.Gid
			&& left.Gecos == right.Gecos && left.Dir == right.Dir && left.Shell == right.Shell
			&& left.SystemUser == right.SystemUser;
		}

		public static bool operator != (PasswdInfo left, PasswdInfo right)
		{
			return !(left == right);
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
