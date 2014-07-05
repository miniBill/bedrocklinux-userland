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

#define dynarray(T) dynarray_##T

#define inline_dynarray(T) { \
	int size; \
	T * array; \
}

#define def_dynarray(T) struct dynarray(T) inline_dynarray(T);

struct tables {
	struct inline_dynarray(uid_t) uid;
	struct inline_dynarray(gid_t) gid;
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

void merge_etcfiles(const char* master, const char* client, struct tables* tables)
{

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

