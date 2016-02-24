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
			public static object GetFieldValue(this object O, string FieldName, BindingFlags BF = Runtime.DefaultFlags) {
				FieldInfo FInfo = O.GetFieldInfo(FieldName, BF);
				if (FInfo == null)
					throw new Exception("Could not find field '" + FieldName + "'");
				return FInfo.GetValue(O);
			}

			public static void SetFieldValue(this object O, string FieldName, object Val, BindingFlags BF = Runtime.DefaultFlags) {
				FieldInfo FInfo = O.GetFieldInfo(FieldName, BF);
				if (FInfo == null)
					throw new Exception("Could not find field '" + FieldName + "' in " + O);
				FInfo.SetValue(O, Val);
			}

			public static object SetFieldValue(this object O, object AnonType, BindingFlags BF = Runtime.DefaultFlags) {
				PropertyInfo[] Props = AnonType.GetType().GetProperties(BF);

				for (int i = 0; i < Props.Length; i++)
					O.SetFieldValue(Props[i].Name, Props[i].GetValue(AnonType));

				return O;
			}
		}
	}
}