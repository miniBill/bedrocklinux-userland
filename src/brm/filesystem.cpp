#include "filesystem.h"

#include <sys/stat.h>    // stat

using namespace std;

//Stats a path, returns the kind of object.
stat_result lookup_path(const string& path)
{
	struct stat s;

	if (stat(path.c_str(), &s) == -1) {
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
