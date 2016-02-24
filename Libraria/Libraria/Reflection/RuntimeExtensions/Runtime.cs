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
		public static partial class RuntimeExtensions {
			public static Type[] GetParamTypes(this MethodInfo MI) {
				ParameterInfo[] PI = MI.GetParameters();
				List<Type> Types = new List<Type>();
				if (MI.CallingConvention.HasFlag(CallingConventions.HasThis))
					Types.Add(MI.ReflectedType);
				for (int i = 0; i < PI.Length; i++)
					Types.Add(PI[i].ParameterType);
				return Types.ToArray();
			}
		}
	}
}