#ifndef PARSER_H
#define PARSER_H

#include <vector>
#include <ostream>
#include <unordered_map>

#include "shadow.h"

class passwd_info {
public:
	//The names come from the passwd structure
	const std::string _name;
	const uid_t _id;
	const gid_t _gid;
	const std::string _gecos; //User info
	const std::string _dir;
	const std::string _shell;

	passwd_info(std::string name, uid_t id, gid_t gid,
			  std::string gecos, std::string dir, std::string shell) :
		_name(name), _id(id), _gid(gid),
		_gecos(gecos), _dir(dir), _shell(shell) {}

	friend std::ostream& operator<<(std::ostream& stream, const passwd_info& info);
};

class group_info {
public:
	//The names come from the group structure
	const std::string _name;
	const gid_t _id;
	const std::vector<std::string> _mem; //Members

	group_info(std::string name, gid_t id,
			  std::vector<std::string>&& mem) :
		_name(name), _id(id), _mem(mem) {}

	friend std::ostream& operator<<(std::ostream& stream, const group_info& info);
};

class shadow_info {
public:
	const std::string _name;
	const std::string _pwd;
	const long _lstchg;
	const long _min;
	const long _max;
	const long _warn;
	const long _inact;
	const long _expire;
	const unsigned long _flag;

	shadow_info(std::string name, std::string pwd, long lstchg,
				long min, long max, long warn, long inact, long expire,
				unsigned long flag) :
		_name(name), _pwd(pwd), _lstchg(lstchg),
		_min(min), _max(max), _warn(warn), _inact(inact), _expire(expire),
		_flag(flag) {}

	friend std::ostream& operator<<(std::ostream& stream, const shadow_info& info);
};

std::unordered_map<std::string, passwd_info> read_passwd(const std::string& path);
std::unordered_map<std::string,  group_info> read_group (const std::string& path);
std::unordered_map<std::string, shadow_info> read_shadow(const std::string& path);

#endif // PARSER_H
