using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Libraria.IO {
	public static class PathExtended {
		public static bool IsSameFile(string PathA, string PathB) {
			PathA = PathA.Replace('/', '\\');
			PathB = PathB.Replace('/', '\\');
			return (PathA == PathB) || (PathA.EndsWith(PathB) || PathB.EndsWith(PathA));
		}
	}
}