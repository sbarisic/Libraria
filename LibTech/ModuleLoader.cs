using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace LibTech {
	public enum ModuleEvent {
		NONE = 0,
		UPDATE, RENDER
	}

	public interface IModule {
		void Init(params object[] Args);
		void Event(ModuleEvent Evt, params object[] Args);
	}

	public class ModuleLoader {
		public static IModule LoadModule(string Pth) {
			Pth = Path.GetFullPath(Pth);
			if (!Pth.ToLower().EndsWith(".dll"))
				Pth += ".dll";

			if (!File.Exists(Pth))
				throw new FileNotFoundException("Could not find module", Pth);

			Assembly Asm = Assembly.LoadFile(Pth);
			Type[] Types = Asm.GetExportedTypes();
			Types = Types.Where(T => T.GetInterfaces().Contains(typeof(IModule))).ToArray();
			if (Types.Length != 1)
				throw new Exception("Assembly must implement exactly one IModule");

			IModule Mod = (IModule)Activator.CreateInstance(Types[0]);
			return Mod;
		}

		public static IModule LoadModule(params string[] Pth) {
			return LoadModule(Path.Combine(Pth));
		}

		public static void UnloadModule(IModule Mod) {
		}
	}
}