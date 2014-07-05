/*
 * trie.h
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 */

#ifndef TRIE_H
#define TRIE_H

#include "indent.h"

#include <string.h>

void indent(int i);

void dump_char(const char* value);
void dump_int(int * value);

#define trie(T) trie_##T

#define def_trie(T) struct trie(T); \
struct trie(T) { \
	struct trie(T)* children[256]; \
	T * value; \
};

#define trie_create(T) trie_##T##_create

#define def_trie_create(T) \
static struct trie(T)* trie_create(T)() \
{ \
	struct trie(T)* toret = (struct trie(T)*)malloc(sizeof(struct trie(T))); \
	memset(toret->children, 0, 256 * sizeof(struct trie(T)*)); \
	return toret; \
}

#define trie_lookup_string(T) trie_##T##_lookup_string

#define def_trie_lookup_string(T) \
static T* trie_lookup_string(T)(struct trie(T)* trie, const char* key) \
{ \
	if (key[0] == '\0') { \
		return trie->value; \
	} \
	if(trie->children[(int)key[0]] == NULL) { \
		return NULL; \
	} \
	return trie_lookup_string(trie->children[(int)key[0]], key + 1); \
}

#define trie_add_string(T) trie_##T##_add_string

#define def_trie_add_string(T) \
static void trie_add_string(T)(struct trie(T)* trie, const char* key, T* value) \
{ \
	if (key[0] == '\0') { \
		trie->value = value; \
		return; \
	} \
	if(trie->children[(int)key[0]] == NULL) { \
		trie->children[(int)key[0]] = trie_create(T)(); \
	} \
	trie_add_string(T)(trie->children[(int)key[0]], key + 1, value); \
}

#define trie_dump(T) trie_##T##_dump
#define _trie_dump(T) _trie_##T##_dump

#define def_trie_dump(T) \
static void _trie_dump(T)(struct trie(T)* trie, int i) \
{ \
	int j; \
	if (trie->value != NULL) { \
		dump_##T(trie->value); \
	} else { \
		printf("<nothing>"); \
	} \
	printf("\n"); \
	char any = 0; \
	for (j = 0; j < 256; j++) { \
		struct trie(T) * child = trie->children[j]; \
		if (child != NULL) { \
			if (!any) { \
				any = 1; \
				indent(i); printf("{\n"); \
			} \
			indent(i); printf("%03d %c: ", j, j); \
			_trie_dump(T)(child, i+7); \
		} \
	} \
	if (any) { \
		indent(i); printf("}\n"); \
	} \
} \
static void trie_dump(T)(struct trie(T)* trie) \
{ \
	printf("root:"); _trie_dump(T)(trie, 0);\
}

#endif // TRIE_H
