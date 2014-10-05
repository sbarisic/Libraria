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
			public static T ToStruct<T>(this IntPtr StructPtr) where T : struct {
				return (T)Marshal.PtrToStructure(StructPtr, typeof(T));
			}
		}
	}
}