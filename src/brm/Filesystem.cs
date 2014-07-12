using Mono.Unix;

namespace Brm
{
	public class Filesystem
	{
		/// <summary>
		/// Stats a path, returns the kind of object.
		/// </summary>
		/// <returns>The kind of object.</returns>
		/// <param name="path">A path.</param>
		public static StatResult LookupPath (string path)
		{
			UnixFileSystemInfo entry = UnixFileInfo.GetFileSystemEntry (path);
			if (!entry.Exists)
				return StatResult.Nothing;
			switch (entry.FileType) {
			case FileTypes.RegularFile:
				return StatResult.File;
			case FileTypes.Directory:
				return StatResult.Directory;
			default:
				return StatResult.Other;
			}
		}
	}
}