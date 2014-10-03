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
			public static Delegate CreateRaiseDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.GetRaiseMethod(NonPublic).ToDelegate();
			}

			public static Delegate CreateRaiseDelegate(this object O, string EventName, bool NonPublic = true) {
				return O.GetEventInfo(EventName).CreateRaiseDelegate(NonPublic);
			}

			public static Delegate CreateAddDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.GetAddMethod(NonPublic).ToDelegate();
			}

			public static Delegate CreateAddDelegate(this object O, string EventName, bool NonPublic = true) {
				return O.GetEventInfo(EventName).CreateAddDelegate(NonPublic);
			}

			public static Delegate CreateRemoveDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.GetRemoveMethod(NonPublic).ToDelegate();
			}

			public static Delegate CreateRemoveDelegate(this object O, string EventName, bool NonPublic = true) {
				return O.GetEventInfo(EventName).CreateRemoveDelegate(NonPublic);
			}

			public static Delegate CreateCallDelegate(this EventInfo EI, bool NonPublic = true) {
				return EI.EventHandlerType.GetMethod("Invoke").ToDelegate();
			}

			public static Delegate CreateCallDelegate(this object O, string EventName, bool NonPublic = true) {
				return O.GetEventInfo(EventName).CreateCallDelegate(NonPublic);
			}

			public static EventInfo GetEventInfo(this object O, string EventName) {
				return O.GetType().GetEvent(EventName);
			}

			public static object CallEvent(this EventInfo EI, params object[] Params) {
				return EI.CreateCallDelegate().DynamicInvoke(Params);
			}

			public static object CallEvent(this object O, string EventName, params object[] Params) {
				List<object> Args = new List<object>();
				Args.Add(O.GetFieldValue(EventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField));
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