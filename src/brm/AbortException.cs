/*
 * AbortException.cs
 *
 *      This program is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU General Public License
 *      version 2 as published by the Free Software Foundation.
 *
 * Copyright (c) 2014 Leonardo Taglialegne <cmt.miniBill@gmail.com>
 *
 */

using System;
using Brm;

namespace Brm
{
	class AbortException : Exception
	{
		public AbortException (string message) : base(message)
		{
		}
	}
}
