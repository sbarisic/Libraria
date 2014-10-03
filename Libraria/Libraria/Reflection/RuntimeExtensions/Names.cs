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
			public static string CreateDelegateName(this MethodInfo MI) {
				return CreateDelegateName(MI.ReturnType, MI.GetParamTypes());
			}

			public static string CreateMethodName(this MethodInfo MI) {
				string Name = "Method?" + MI.ReturnType.Name + "?" + MI.Name + "?";
				foreach (var PType in GetParamTypes(MI))
					Name += PType.Name;
				return Name;
			}
		}
	}
}