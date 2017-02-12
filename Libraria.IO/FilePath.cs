using System;
using System.Collections.Generic;
using System.Text;
using System.IO;using System.Reflection;

namespace Libraria.IO {
	public static class FilePath {
		public static string Normalize(string Pth) {
			return Pth.Replace('/', '\\');
		}

		public static string GetEntryAssemblyPath() {
			return Normalize(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
		}

		public static void EnsureDirExists(string DirName) {
			if (!Directory.Exists(DirName))
				Directory.CreateDirectory(DirName);
		}
	}
}
