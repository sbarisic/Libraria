using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Libraria.Reflection {
	public static class Assemblies {
		public static IEnumerable<Assembly> GetAssemblies(bool ReflectionOnly = false) {
			if (ReflectionOnly)
				return AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public static Assembly GetEntryAssembly() {
			return Assembly.GetEntryAssembly();
		}

		public static Assembly LoadFrom(string File, bool ReflectionOnlyLoad = false) {
			if (ReflectionOnlyLoad)
				return Assembly.ReflectionOnlyLoadFrom(File);
			return Assembly.LoadFrom(File);
		}
		
		public static IEnumerable<T> LoadInterfaces<T>(Assembly Asm) {
			Type[] Types = Asm.GetTypes();
			List<T> TypeInstances = new List<T>();

			for (int i = 0; i < Types.Length; i++) {
				Type TT = Types[i];

				if (TT.GetInterfaces().Contains(typeof(T)))
					TypeInstances.Add((T)Activator.CreateInstance(TT));
			}

			return TypeInstances;
		}

		public static IEnumerable<T> LoadInterfaces<T>(string File) {
			return LoadInterfaces<T>(LoadFrom(File));
		}
	}
}