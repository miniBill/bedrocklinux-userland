#ifndef PARSER_H
#define PARSER_H

#include <vector>
#include <ostream>
#include <unordered_map>

class user_info {
public:
	//The names come from the passwd structure
	const std::string _name;
	const uid_t _uid;
	const gid_t _gid;
	const std::string _gecos; //User info
	const std::string _dir;
	const std::string _shell;

	user_info(std::string name, uid_t uid, gid_t gid,
			  std::string gecos, std::string dir, std::string shell) :
		_name(name), _uid(uid), _gid(gid),
		_gecos(gecos), _dir(dir), _shell(shell) {}

	friend std::ostream& operator<<(std::ostream& stream, const user_info& info);
};

class group_info {
public:
	//The names come from the group structure
	const std::string _name;
	const gid_t _gid;
	const std::vector<std::string> _mem; //Members

	group_info(std::string name, gid_t gid,
			  std::vector<std::string>&& mem) :
		_name(name), _gid(gid), _mem(mem) {}

	friend std::ostream& operator<<(std::ostream& stream, const group_info& info);
};

std::unordered_map<std::string, user_info> read_passwd(const std::string& path);
std::unordered_map<std::string, group_info> read_group(const std::string& path);

#endif // PARSER_H
