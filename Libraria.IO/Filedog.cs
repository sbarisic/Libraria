using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Libraria.IO {
	public delegate void OnFileChangedAction(string Extension, string FullPath, string Name, WatcherChangeTypes ChangeType);

	public class Filedog {
		FileSystemWatcher FSW;
		Dictionary<string, OnFileChangedAction> ExtWatchers;
		Dictionary<string, OnFileChangedAction> FileWatchers;

		public Filedog(string BaseDirectory) {
			FSW = new FileSystemWatcher();
			FSW.Path = BaseDirectory;
			FSW.Filter = "";
			FSW.IncludeSubdirectories = true;
			FSW.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

			FSW.Changed += OnChanged;
			//FSW.Created += OnChanged;
			//FSW.Renamed += OnChanged;
			//FSW.Deleted += OnChanged;
			FSW.Error += OnError;

			ExtWatchers = new Dictionary<string, OnFileChangedAction>();
			FileWatchers = new Dictionary<string, OnFileChangedAction>();

			FSW.EnableRaisingEvents = true;
		}

		private void OnError(object S, ErrorEventArgs E) {
			throw new Exception("Error");
		}

		public void OnFileChanged(string FullPath, OnFileChangedAction A) {
			FullPath = FilePath.Normalize(Path.GetFullPath(FullPath));

			if (FileWatchers.ContainsKey(FullPath))
				throw new Exception("File already watched");

			FileWatchers.Add(FullPath, A);
		}

		public void OnExtChanged(string Ext, OnFileChangedAction A) {
			if (ExtWatchers.ContainsKey(Ext))
				throw new Exception("Extension already watched");

			ExtWatchers.Add(Ext, A);
		}

		private void OnChanged(object S, FileSystemEventArgs E) {
			string Ext = Path.GetExtension(E.FullPath) ?? "";

			if (FileWatchers.ContainsKey(E.FullPath))
				FileWatchers[E.FullPath](Ext, E.FullPath, E.Name, E.ChangeType);

			if (ExtWatchers.ContainsKey(Ext))
				ExtWatchers[Ext](Ext, E.FullPath, E.Name, E.ChangeType);
		}
	}
}
