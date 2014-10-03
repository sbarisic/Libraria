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
			public static object GetFieldValue(this object O, string FieldName, BindingFlags BF) {
				return O.GetType().GetField(FieldName, BF).GetValue(O);
			}

			public static object GetFieldValue(this Type T, string FieldName, BindingFlags BF) {
				//return (T.GetMember(FieldName, MemberTypes.All, BF)[0]); TODO
				return null;
			}
		}
	}
}