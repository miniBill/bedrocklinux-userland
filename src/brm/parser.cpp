#include "parser.h"

#include <sstream>
#include <iostream>
#include <fstream>
#include <tuple>

using namespace std;

ostream& operator<<(ostream& stream, const user_info& info) {
	return stream
		<< info._name  << ':'
		<< 'x'         << ':'
		<< info._uid   << ':'
		<< info._gid   << ':'
		<< info._gecos << ':'
		<< info._dir   << ':'
		<< info._shell;
}

ostream& operator<<(ostream& stream, const group_info& info) {
	stream
		<< info._name  << ':'
		<< 'x'         << ':'
		<< info._gid   << ':';
	for (size_t i = 0; i < info._mem.size(); i++) {
		if (i != 0) {
			stream << ',';
		}
		stream << info._mem[i];
	}
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

unordered_map<string, user_info> read_passwd(const string& path)
{
	unordered_map<string, user_info> users;
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
