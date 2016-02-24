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
			public static Delegate CreateDelegate(this MethodInfo MI) {
				if (MI == null)
					throw new ArgumentException("MethodInfo MI cannot be null", "MI");

				return Delegate.CreateDelegate(Runtime.CreateDelegateType(MI.ReturnType, MI.GetParamTypes()), MI);
			}

			public static Delegate CreateDelegate(this Delegate D) {
				return D.Method.CreateDelegate();
			}

			public static Delegate CreateGetDelegate(this PropertyInfo PI, bool NonPublic = true) {
				return PI.GetGetMethod(NonPublic).CreateDelegate();
			}

			public static Delegate CreateSetDelegate(this PropertyInfo PI, bool NonPublic = true) {
				return PI.GetSetMethod(NonPublic).CreateDelegate();
			}

			public static T ToDelegate<T>(this IntPtr FuncPtr) where T : class {
				T Ret = Marshal.GetDelegateForFunctionPointer(FuncPtr, typeof(T)) as T;
				if (Ret == null)
					throw new Exception("Cannot convert to " + typeof(T));
				return Ret;
			}
		}
	}
}