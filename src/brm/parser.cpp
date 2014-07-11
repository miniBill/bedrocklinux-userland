#include "parser.h"

#include <sstream>
#include <iostream>
#include <fstream>
#include <tuple>

using namespace std;

ostream& operator<<(ostream& stream, const passwd_info& info) {
	return stream
		<< info._name  << ':'
		<< 'x'         << ':'
		<< info._id   << ':'
		<< info._gid   << ':'
		<< info._gecos << ':'
		<< info._dir   << ':'
		<< info._shell;
}

ostream& operator<<(ostream& stream, const group_info& info) {
	stream
		<< info._name  << ':'
		<< 'x'         << ':'
		<< info._id   << ':';
	for (size_t i = 0; i < info._mem.size(); i++) {
		if (i != 0) {
			stream << ',';
		}
		stream << info._mem[i];
	}
	return stream;
}

template<typename T>
static void print_nonzero(ostream& stream, T value)
{
	stream << ':';
	if(value != 0)
		stream << value;
}

ostream& operator<<(ostream& stream, const shadow_info& info) {
	stream
		<< info._name   << ':'
		<< info._pwd;
	print_nonzero(stream, info._lstchg);
	stream
		<< ':' << info._min
		<< ':' << info._max;
	print_nonzero(stream, info._warn);
	print_nonzero(stream, info._inact);
	print_nonzero(stream, info._expire);
	print_nonzero(stream, info._flag);
	return stream;
}

// Gets a field from a semicolon-separated line
// Errors and exits on failure
static string get_field(istringstream& stream, const string& name, int line_number)
{
	string toret;
	if (!getline(stream, toret, ':')) {
		cerr << "Couldn't read " << name << " field on line " << line_number << endl;
		exit(1);
	}
	return toret;
}

std::unordered_map<string, passwd_info> read_passwd(const string& path)
{
	unordered_map<string, passwd_info> users;
	ifstream passwd(path + "/passwd");

	if(!passwd)
		exit(1);

	string line;
	for (int line_number = 1; getline(passwd, line); line_number++) {
		istringstream iss(line);
		string name   = get_field(iss, "name",     line_number);
		string shadow = get_field(iss, "password", line_number);
		string s_uid  = get_field(iss, "uid",      line_number);
		string s_gid  = get_field(iss, "gid",      line_number);
		string gecos  = get_field(iss, "GECOS",    line_number);
		string dir    = get_field(iss, "home dir", line_number);
		string shell  = get_field(iss, "shell",    line_number);

		if(shadow != "x") {
			cerr << "Unexpected \"" << shadow
				 << "\" instead of \"x\" as second field on line "
				 << line_number << endl;
			exit(1);
		}

		//TODO: check for errors here
		uid_t uid = atoi(s_uid.c_str());
		gid_t gid = atoi(s_gid.c_str());

		string key = name;

		users.emplace(piecewise_construct, forward_as_tuple(move(key)),
					  forward_as_tuple(move(name), uid, gid, move(gecos), move(dir), move(shell)));
	}

	return users;
}

unordered_map<string, group_info> read_group(const string& path)
{
	unordered_map<string, group_info> groups;
	ifstream group(path + "/group");

	if(!group)
		exit(1);

	string line;
	for (int line_number = 1; getline(group, line); line_number++) {
		istringstream iss(line);
		string name   = get_field(iss, "name",     line_number);
		string shadow = get_field(iss, "password", line_number);
		string s_gid  = get_field(iss, "gid",      line_number);
		string s_mem; getline(iss, s_mem); //We don't care if empty

		if(shadow != "x") {
			cerr << "Unexpected \"" << shadow
				 << "\" instead of \"x\" as second field on line "
				 << line_number << endl;
			exit(1);
		}

		//TODO: check for errors here
		gid_t gid = atoi(s_gid.c_str());

		vector<string> mem;
		istringstream smems(s_mem);
		string member;
		while(getline(smems, member)) {
			mem.push_back(move(member));
		}

		string key = name;

		groups.emplace(piecewise_construct, forward_as_tuple(move(key)),
					   forward_as_tuple(move(name), gid, move(mem)));
	}

	return groups;
}

std::unordered_map<string, shadow_info> read_shadow(const string& path)
{
	unordered_map<string, shadow_info> users;
	ifstream shadow(path + "/shadow");

	if(!shadow)
		exit(1);

	string line;
	for (int line_number = 1; getline(shadow, line); line_number++) {
		istringstream iss(line);
		string name     = get_field(iss, "name",          line_number);
		string pwd      = get_field(iss, "password",      line_number);
		string s_lstchg = get_field(iss, "last change",   line_number);
		string s_min    = get_field(iss, "min time",      line_number);
		string s_max    = get_field(iss, "max time",      line_number);
		string s_warn   = get_field(iss, "warn time",     line_number);
		string s_inact  = get_field(iss, "inactive time", line_number);
		string s_expire = get_field(iss, "expire time",   line_number);
		string s_flag; getline(iss, s_flag); //We don't care if empty

		//TODO: check for errors here
		long lstchg = atol(s_lstchg.c_str());
		long min    = atol(s_min.c_str());
		long max    = atol(s_max.c_str());
		long warn   = atol(s_warn.c_str());
		long inact  = atol(s_inact.c_str());
		long expire = atol(s_expire.c_str());
		unsigned long flag = atol(s_flag.c_str());

		string key = name;

		users.emplace(piecewise_construct, forward_as_tuple(move(key)),
					  forward_as_tuple(move(name), move(pwd),
									   lstchg, min, max, warn, inact, expire, flag));
	}

	return users;
}
