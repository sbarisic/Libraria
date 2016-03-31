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
			public static Delegate CreateRaiseDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.GetRaiseMethod(NonPublic).CreateDelegate();
			}

			public static Delegate CreateAddDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.GetAddMethod(NonPublic).CreateDelegate();
			}

			public static Delegate CreateRemoveDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.GetRemoveMethod(NonPublic).CreateDelegate();
			}

			public static Delegate CreateCallDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.EventHandlerType.GetMethod("Invoke").CreateDelegate();
			}

			public static EventInfo GetEventInfo(this object O, string EventName, BindingFlags BF = ReflectionRuntime.DefaultFlags) {
				return O.GetType().GetEvent(EventName, BF);
			}

			public static FieldInfo GetFieldInfo(this object O, string FieldName, BindingFlags BF = ReflectionRuntime.DefaultFlags) {
				return O.GetType().GetField(FieldName, BF);
			}

			public static object CallEvent(this EventInfo EI, params object[] Params) {
				return EI.CreateCallDelegate().DynamicInvoke(Params);
			}

			public static object CallEvent(this object O, string EventName, params object[] Params) {
				List<object> Args = new List<object>();
				Args.Add(O.GetFieldValue(EventName));
				Args.AddRange(Params);
				return O.GetType().GetEvent(EventName).CallEvent(Args.ToArray());
			}

			public static object CallEvent(this Type T, string EventName, BindingFlags BF, params object[] Params) {
				List<object> Args = new List<object>();
				Args.Add(T.GetFieldValue(EventName, BF));
				Args.AddRange(Params);
				return T.GetEvent(EventName).CallEvent(Args.ToArray());
			}
		}
	}
}