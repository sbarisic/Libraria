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
			public static string CreateDelegateName(Type ReturnType, Type[] Args) {
				string Name = "Delegate?" + ReturnType.Name + "??";
				foreach (var PType in Args)
					Name += PType.Name;
				return Name;
			}
		}
	}
}