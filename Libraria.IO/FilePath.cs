using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Libraria.IO {
	public static class FilePath {
		public static string GetProcessExePath() {
			return Assembly.GetEntryAssembly().Location;
		}

		public static string GetProcessExeDirectory() {
			return Path.GetDirectoryName(GetProcessExePath());
		}

		public static string Normalize(string Pth) {
			if (!Pth.Contains("/"))
				return Pth;
			return Pth.Replace('/', '\\').Trim();
		}

		public static string GetEntryAssemblyPath() {
			return Normalize(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
		}

		public static void EnsureDirExists(string DirName) {
			if (!Directory.Exists(DirName))
				Directory.CreateDirectory(DirName);
		}

		public static bool TryOpenFile(string FilePath, FileAccess Access) {
			try {
				FileStream FStream = File.Open(FilePath, FileMode.Open, Access);
				FStream.Dispose();

				return true;
			} catch (Exception) {
			}
			return false;
		}
	}
}
