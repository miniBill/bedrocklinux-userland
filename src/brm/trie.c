/*
 * trie.c
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 */


#include "trie.h"

#include <stdio.h>

void dump_char(const char* value)
{
	printf("\"%s\"", value);
}

void dump_int(int* value)
{
	printf("%d", *value);
}

void indent(int i)
{
	while(i --> 0)
		printf(" ");
}
