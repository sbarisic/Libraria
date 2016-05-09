using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security.Policy;
using System.IO;

namespace Libraria.Reflection {
	public class Plugin {
		public static IEnumerable<Plugin> LoadAll(string Dir, string SearchPattern = "*.dll",
			SearchOption Search = SearchOption.AllDirectories) {

			string[] Files = Directory.GetFiles(Dir, SearchPattern, Search);
			for (int i = 0; i < Files.Length; i++)
				yield return new Plugin(Files[i]);
		}

		public Assembly Assembly;

		public Plugin(string PluginPath) {
			PluginPath = Path.GetFullPath(PluginPath);
			Assembly = Assemblies.LoadFrom(PluginPath);
		}

		public IEnumerable<T> GetInterfaces<T>() {
			return Assemblies.LoadInterfaces<T>(Assembly);
		}
	}
}
