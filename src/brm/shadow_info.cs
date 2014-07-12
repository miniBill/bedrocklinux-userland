using System;
using System.Collections.Generic;
using System.IO;
using Brm;
using System.Linq;

public class shadow_info
{
	public string _name;
	public string _pwd;
	public long _lstchg;
	public long _min;
	public long _max;
	public long _warn;
	public long _inact;
	public long _expire;
	public ulong _flag;

	public shadow_info (string name, string pwd, long lstchg,
	                    long min, long max, long warn, long inact, long expire,
	                    ulong flag)
	{
		_name = name;
		_pwd = pwd;
		_lstchg = lstchg;
		_min = min;
		_max = max;
		_warn = warn;
		_inact = inact;
		_expire = expire;
		_flag = flag;
	}

	static void print_nonzero (StreamWriter stream, long value)
	{
		stream.Write (':');
		if (value != 0)
			stream.Write (value);
	}

	static void print_nonzero (StreamWriter stream, ulong value)
	{
		stream.Write (':');
		if (value != 0)
			stream.Write (value);
	}

	public override string ToString ()
	{
		return string.Format ("[shadow_info]");
		/*stream
		<< info._name << ':'
		<< info._pwd;
	print_nonzero(stream, info._lstchg);
	stream
		<< ':' << info._min
		<< ':' << info._max;
	print_nonzero(stream, info._warn);
	print_nonzero(stream, info._inact);
	print_nonzero(stream, info._expire);
	print_nonzero(stream, info._flag);
	return stream;*/
	}
};
