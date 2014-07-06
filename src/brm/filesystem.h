#ifndef FILESYSTEM_H
#define FILESYSTEM_H

#include <string>

enum stat_result {
	File,
	Directory,
	Other,
	Nothing // Doesn't exist
};

//Stats a path, returns the kind of object.
stat_result lookup_path(const std::string &path);

#endif // FILESYSTEM_H
