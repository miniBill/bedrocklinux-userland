/*
 * ShadowInfo.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 */


using System.Text;

namespace Brm
{
	public class ShadowInfo
	{
		public string Name { get; private set; }

		public string Pwd { get; private set; }

		public long Lstchg { get; private set; }

		public long Min { get; private set; }

		public long Max { get; private set; }

		public long Warn { get; private set; }

		public long Inact { get; private set; }

		public long Expire { get; private set; }

		public ulong Flag { get; private set; }

		public ShadowInfo (string name, string pwd, long lstchg,
		                   long min, long max, long warn, long inact, long expire,
		                   ulong flag)
		{
			Name = name;
			Pwd = pwd;
			Lstchg = lstchg;
			Min = min;
			Max = max;
			Warn = warn;
			Inact = inact;
			Expire = expire;
			Flag = flag;
		}

		static void print_nonzero (StringBuilder sb, long value)
		{
			sb.Append (':');
			if (value != 0)
				sb.Append (value);
		}

		static void print_nonzero (StringBuilder sb, ulong value)
		{
			sb.Append (':');
			if (value != 0)
				sb.Append (value);
		}

		public override string ToString ()
		{
			var sb = new StringBuilder ();
			sb.Append (Name);
			sb.Append (':');
			sb.Append (Pwd);
			print_nonzero (sb, Lstchg);
			sb.Append (':');
			sb.Append (Min);
			sb.Append (':');
			sb.Append (Max);
			print_nonzero (sb, Warn);
			print_nonzero (sb, Inact);
			print_nonzero (sb, Expire);
			print_nonzero (sb, Flag);
			return sb.ToString ();
		}
	}
}