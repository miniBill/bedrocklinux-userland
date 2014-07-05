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

int main(int argc, char* argv[])
{
	// Sanity check - ensure there are sufficient arguments
	if (argc < 2) {
		fprintf(stderr, "No client path specified, aborting\n");
		exit(1);
	}

	fprintf(stderr, "Implement me!\n");
	return -1;
}

