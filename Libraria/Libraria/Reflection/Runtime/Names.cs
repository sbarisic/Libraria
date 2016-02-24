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
			public static string CreateDelegateName(MethodInfo MI) {
				return CreateDelegateName(MI.ReturnType, MI.GetParamTypes());
			}

			public static string CreateDelegateName(Type ReturnType, Type[] Args) {
				string Name = "Delegate?" + ReturnType.FullName + "??";

				for (int i = 0; i < Args.Length; i++) {
					Name += Args[i].FullName;
					if (i + 1 < Args.Length)
						Name += "?";
				}

				return Name;
			}

			public static string CreateMethodName(MethodInfo MI) {
				string Name = "Method?" + MI.ReturnType.FullName + "??" + MI.Name + "??";

				Type[] Args = MI.GetParamTypes();
				for (int i = 0; i < Args.Length; i++) {
					Name += Args[i].FullName;
					if (i + 1 < Args.Length)
						Name += "?";
				}

				return Name;
			}
		}
	}
}