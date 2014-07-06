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

	tables() = default;
	tables(tables&& other) = default;
	tables(const tables& other) = delete;
	~tables() = default;
};

tables merge_etcfiles(const std::string& master, const std::string& client)
{
	tables toret;

	unordered_map<string, group_info> master_group = read_group(master);
	unordered_map<string, group_info> client_group = read_group(client);

	unordered_map<string, passwd_info> master_passwd = read_passwd(master);
	unordered_map<string, passwd_info> client_passwd = read_passwd(client);

	unordered_map<string, shadow_info> master_shadow = read_shadow(master);
	unordered_map<string, shadow_info> client_shadow = read_shadow(client);

	return toret;
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

