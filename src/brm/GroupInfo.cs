/*
 * GroupInfo.cs
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
using System.Text;

namespace Brm
{
	public class GroupInfo
	{
		public string Name { get; private set; }

		public UInt32 Id { get; private set; }

		public IReadOnlyCollection<string> Members { get; private set; }

		public bool SystemGroup { get; private set; }

		public GroupInfo (string name, UInt32 id, IReadOnlyCollection<string> members, bool systemGroup)
		{
			Name = name;
			Id = id;
			Members = members;
			SystemGroup = systemGroup;
		}

		public GroupInfo WithId (uint newId)
		{
			return new GroupInfo (Name, newId, Members, SystemGroup);
		}

		public override string ToString ()
		{
			var sb = new StringBuilder ();
			sb.Append (Name);
			sb.Append (":x:");
			sb.Append (Id);
			sb.Append (':');
			bool first = true;
			foreach (string member in Members) {
				if (first)
					first = false;
				else
					sb.Append (',');
				sb.Append (member);
			}
			return sb.ToString ();
		}
	}
}
