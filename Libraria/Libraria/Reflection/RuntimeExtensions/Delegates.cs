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
			public static Delegate ToDelegate(this MethodInfo MI) {
				if (MI == null)
					throw new ArgumentException("MethodInfo MI cannot be null", "MI");
				return Delegate.CreateDelegate(CreateDelegateType(MI.ReturnType, MI.GetParamTypes()), MI);
			}

			public static Delegate CreateGetDelegate(this PropertyInfo PI, bool NonPublic = true) {
				return PI.GetGetMethod(NonPublic).ToDelegate();
			}

			public static Delegate CreateSetDelegate(this PropertyInfo PI, bool NonPublic = true) {
				return PI.GetSetMethod(NonPublic).ToDelegate();
			}
		}
	}
}