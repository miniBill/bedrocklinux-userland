/*
 * Filesystem.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 */

using Mono.Unix;

namespace Brm
{
	public static class Filesystem
	{
		/// <summary>
		/// Stats a path, returns the kind of object.
		/// </summary>
		/// <returns>The kind of object.</returns>
		/// <param name="path">A path.</param>
		public static StatResult LookupPath (string path)
		{
			UnixFileSystemInfo entry = UnixFileSystemInfo.GetFileSystemEntry (path);
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