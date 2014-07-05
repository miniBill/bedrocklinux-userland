/*
 * brm.c
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

#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>
#include <errno.h>
#include <stdio.h>
#include <stdlib.h>

#include "trie.h"

#define safe_array(T) safe_array_##T

#define _safe_array(T) { \
	int size; \
	T * array; \
}

#define def_safe_array(T) struct safe_array(T) _safe_array(T);

def_safe_array(uid_t);
def_safe_array(gid_t);

struct tables {
	struct safe_array(uid_t) uid;
	struct safe_array(gid_t) gid;
};

enum stat_result {
	File,
	Directory,
	Other,
	Nothing // Doesn't exist
};

/*
 * Lookup a path, returns the kind of object.
 */
enum stat_result lookup_path(const char* path)
{
	struct stat s;

	if (stat(path, &s) == -1) {
		if (errno != ENOENT) {
			perror("stat");
			exit(1);
		}
		return Nothing;
	}
	if (S_ISDIR(s.st_mode)) {
		return Directory;
	} else if (S_ISREG(s.st_mode)) {
		return File;
	}
	return Other;
}

def_trie(char);
def_trie_create(char);
def_trie_add_string(char);
def_trie_dump(char);

void merge_etcfiles(const char* master, const char* client, struct tables* tables)
{

}

void walk_tree(const char* tree, struct tables* tables)
{

}

int main(int argc, char* argv[])
{
	struct trie(char)* test_trie = trie_create(char)();
	trie_add_string(char)(test_trie, "ciao", "ciao");
	trie_dump(char)(test_trie);
	
	if (argc < 3) {
		fprintf(stderr, "Usage: brm <master etc> <client etc> [<client path>]\n");
		fprintf(stderr, "The optional parameter, if supplied, enables owner fixing.\n");
		exit(1);
	}

	const char* master = argv[1];
	const char* client = argv[2];
	const char* tree   = argv[3];

	if (lookup_path(master) != Directory) {
		fprintf(stderr, "Master path doesn't exist or is not a directory");
		exit(1);
	}

	if (lookup_path(client) != Directory) {
		fprintf(stderr, "Client path doesn't exist or is not a directory");
		exit(1);
	}

	if (tree != NULL && lookup_path(tree) != Directory) {
		fprintf(stderr, "Tree path doesn't exist or is not a directory");
		exit(1);
	}

	struct tables tables;
	merge_etcfiles(master, client, &tables);

	if (tree != NULL) {
		walk_tree(tree, &tables);
	}

	return -1;
}

