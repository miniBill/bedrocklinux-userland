/*
 * brm.cpp
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 * This program will allow merging etcfiles, and walking
 * of a filesystem to fix file owners.
 */

#include <sys/types.h>   // uid_t/gid_t
#include <unistd.h>      // Error codes
#include <errno.h>       // errno
#include <stdio.h>       // perror
#include <stdlib.h>      // exit

#include <iostream>
#include <unordered_map>
#include <set>

#include "parser.h"
#include "filesystem.h"

using namespace std;

class tables {
private:
	unordered_map<uid_t, uid_t> _uid;
	unordered_map<gid_t, gid_t> _gid;
public:
	uid_t translate(uid_t uid)
	{
		return 0;
	}

	tables(unordered_map<uid_t, uid_t>&& uid,
		   unordered_map<uid_t, uid_t>&& gid) :
		_uid(uid), _gid(gid) {}
	tables(tables&& other) = default;
	tables(const tables& other) = delete;
	~tables() = default;
};

template<typename I, typename O>
set<O>&& get_ids(const unordered_map<string, I>& inputs)
{
	set<O> used;
	for (auto& input : inputs)
		used.insert(input.second._id);
	return move(used);
}

void add_group_to(const group_info& info, const string& path)
{
	throw "Implement me";
}

unordered_map<gid_t, gid_t> merge_group(const string master_group_path,
										const unordered_map<string, group_info>& master_group,
										const unordered_map<string, group_info>& client_group)
{
	unordered_map<gid_t, gid_t> toret;
	set<gid_t> used_master = get_ids<group_info, gid_t>(master_group);
	set<gid_t> used_client = get_ids<group_info, gid_t>(client_group);

	vector<pair<gid_t, gid_t>> freelist;

	for (auto& group : client_group) {
		const group_info& info = group.second;
		if (master_group.find(info._name) == master_group.end()) {
			if(used_master.find(info._id) == used_master.end()) {
				add_group_to(info, master_group_path);
				if(toret.find(info._id) == toret.end()) {

				}
			}
		}
	}
	return toret;
}

unordered_map<uid_t, uid_t> merge_passwd(const unordered_map<string, passwd_info>& master_passwd,
										 const unordered_map<string, passwd_info>& client_passwd)
{
	return *(unordered_map<uid_t, uid_t>*)nullptr;
}

tables merge_etcfiles(const std::string& master, const std::string& client)
{
	unordered_map<string, group_info> master_group = read_group(master);
	unordered_map<string, group_info> client_group = read_group(client);

	unordered_map<gid_t, gid_t> gid = merge_group(master + "/group", master_group, client_group);

	unordered_map<string, passwd_info> master_passwd = read_passwd(master);
	unordered_map<string, passwd_info> client_passwd = read_passwd(client);

	unordered_map<uid_t, uid_t> uid = merge_passwd(master_passwd, client_passwd);

	unordered_map<string, shadow_info> master_shadow = read_shadow(master);
	unordered_map<string, shadow_info> client_shadow = read_shadow(client);

	return tables(move(uid), move(gid));
}

void walk_tree(const char* tree, struct tables* tables)
{

}

int main(int argc, char* argv[])
{
	if (argc < 3) {
		fprintf(stderr, "Usage: brm <master etc> <client etc> [<client path>]\n");
		fprintf(stderr, "The optional parameter, if supplied, enables owner fixing.\n");
		exit(1);
	}

	const char* master = argv[1];
	const char* client = argv[2];
	const char* tree   = argv[3];

	if (lookup_path(master) != Directory) {
		fprintf(stderr, "Master path doesn't exist or is not a directory\n");
		exit(1);
	}

	if (lookup_path(client) != Directory) {
		fprintf(stderr, "Client path doesn't exist or is not a directory\n");
		exit(1);
	}

	if (tree != NULL && lookup_path(tree) != Directory) {
		fprintf(stderr, "Tree path doesn't exist or is not a directory\n");
		exit(1);
	}

	tables tables = merge_etcfiles(master, client);

	if (tree != NULL) {
		walk_tree(tree, &tables);
	}

	return -1;
}

