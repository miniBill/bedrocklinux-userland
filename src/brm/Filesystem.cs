using Mono.Unix;

namespace Brm
{
	public class Filesystem
	{
		//Stats a path, returns the kind of object.
		public static stat_result lookup_path (string path)
		{
			UnixFileSystemInfo entry = UnixFileInfo.GetFileSystemEntry (path);
			if (!entry.Exists)
				return stat_result.Nothing;
			switch (entry.FileType) {
			case FileTypes.RegularFile:
				return stat_result.File;
			case FileTypes.Directory:
				return stat_result.Directory;
			default:
				return stat_result.Other;
			}
		}
	}
}