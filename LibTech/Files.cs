using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Libraria.IO;

namespace LibTech {
	public static partial class Engine {
		internal static string BaseFolder;
		internal static string GameFolder;
	}

	public static class Files {
		internal static void Initialize(string BaseFolder) {
			Engine.BaseFolder = BaseFolder;

			FilePath.EnsureDirExists(Engine.BaseFolder);
		}

		public static void SetGameFolder(string GameFolder) {
			Engine.GameFolder = GameFolder;
		}

		public static string GetBaseFolder() {
			return GetFullPath(Engine.BaseFolder);
		}

		public static string GetGameFolder() {
			return GetFullPath(Engine.GameFolder);
		}

		public static string GetFullPath(string Pth) {
			return FilePath.Normalize(Path.GetFullPath(Pth));
		}

		public static string GetRelativePath(string Pth, bool RemoveBaseFolder = true) {
			string Rel = Pth.Replace(FilePath.GetEntryAssemblyPath(), "").TrimStart('\\');

			if (RemoveBaseFolder) {
				if (Rel.StartsWith(Engine.GameFolder + "\\"))
					Rel = Rel.Replace(Engine.GameFolder + "\\", "");
				else if (Rel.StartsWith(Engine.BaseFolder + "\\"))
					Rel = Rel.Replace(Engine.BaseFolder + "\\", "");
			}
			return Rel;
		}

		public static string CombinePath(params string[] Paths) {
			return FilePath.Normalize(Path.Combine(Paths));
		}

		public static string GetFilePath(string RelativePath) {
			string Pth = CombinePath(GetGameFolder(), RelativePath);
			if (!File.Exists(Pth))
				Pth = CombinePath(GetBaseFolder(), RelativePath);

			if (File.Exists(Pth))
				return Pth;
			return null;
		}

		public static string GetDirectoryPath(string RelativePath) {
			string Pth = CombinePath(GetGameFolder(), RelativePath);
			if (!Directory.Exists(Pth))
				Pth = CombinePath(GetBaseFolder(), RelativePath);

			if (Directory.Exists(Pth))
				return Pth;
			return null;
		}

		public static byte[] ReadAllBytes(string RelativePath) {
			RelativePath = GetFilePath(RelativePath);
			if (string.IsNullOrEmpty(RelativePath))
				return null;

			return File.ReadAllBytes(RelativePath);
		}

		public static string ReadAllText(string RelativePath) {
			RelativePath = GetFilePath(RelativePath);
			if (string.IsNullOrEmpty(RelativePath))
				return null;

			return File.ReadAllText(RelativePath);
		}

		public static string GetFileName(string F, bool WithoutExtension = true) {
			if (WithoutExtension)
				return Path.GetFileNameWithoutExtension(F);
			return Path.GetFileName(F);
		}

		public static string[] GetFilesInDirectory(string RelativePath, bool IncludeDirectories = false, string SearchPattern = "*", bool IncludeChildDirectories = true) {
			string[] BaseEntries, GameEntries;
			SearchOption SearchOption = SearchOption.TopDirectoryOnly;
			if (IncludeChildDirectories)
				SearchOption = SearchOption.AllDirectories;

			if (IncludeDirectories) {
				BaseEntries = Directory.GetFileSystemEntries(GetFullPath(CombinePath(GetBaseFolder(), RelativePath)), SearchPattern, SearchOption);
				GameEntries = Directory.GetFileSystemEntries(GetFullPath(CombinePath(GetGameFolder(), RelativePath)), SearchPattern, SearchOption);
			} else {
				BaseEntries = Directory.GetFiles(GetBaseFolder(), SearchPattern, SearchOption);
				GameEntries = Directory.GetFiles(GetGameFolder(), SearchPattern, SearchOption);
			}

			// Normalize names
			for (int i = 0; i < BaseEntries.Length; i++)
				BaseEntries[i] = GetRelativePath(BaseEntries[i]);
			for (int i = 0; i < GameEntries.Length; i++)
				GameEntries[i] = GetRelativePath(GameEntries[i]);

			HashSet<string> Entries = new HashSet<string>();

			foreach (var GameEntry in GameEntries)
				Entries.Add(GetFilePath(GameEntry));
			foreach (var BaseEntry in BaseEntries)
				if (!GameEntries.Contains(BaseEntry))
					Entries.Add(GetFilePath(BaseEntry));

			return Entries.ToArray();
		}
	}
}
