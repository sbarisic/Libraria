using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Libraria {
	namespace Reflection {
		public static partial class Runtime {
			public const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
			internal static AssemblyBuilder AB;
			internal static ModuleBuilder DefMod;
			internal static Dictionary<string, Type> DelegateTypes;

			static Runtime() {
				AB = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Libraria.Test.dll"),
						AssemblyBuilderAccess.RunAndSave);
				DefMod = AB.DefineDynamicModule("Libraria.DefMod");

				DelegateTypes = new Dictionary<string, Type>();
			}
		}
	}
}