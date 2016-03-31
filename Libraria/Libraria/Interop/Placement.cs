using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.IO;
using System.Reflection;
using Libraria.Native;

namespace Libraria.Interop {
	public static unsafe class Placement {
		static Placement() {
			HookHandle.CreateHook<Func<object, uint>, Func<object, object>>(_ToPointer, _ReturnObject);
			HookHandle.CreateHook<Func<uint, object>, Func<uint, uint>>(_ToObject, _ReturnUInt);
		}

		static object _ReturnObject(object Obj) {
			return Obj;
		}

		static uint _ReturnUInt(uint UInt) {
			return UInt;
		}

		static uint _ToPointer(object Obj) {
			return 42;
		}

		static object _ToObject(uint UInt) {
			return null;
		}

		public static IntPtr ToPointer(object Obj) {
			return new IntPtr(_ToPointer(Obj));
		}

		public static T ToObject<T>(IntPtr Ptr) where T : class {
			return (T)_ToObject((uint)Ptr.ToInt32());
		}

		public static int SizeOf<T>() where T : class {
			Type TType = typeof(T);
			IntPtr TypePtr = TType.TypeHandle.Value;
			return Marshal.ReadInt32(TypePtr, 4);
		}

		public static T Allocate<T>(IntPtr Mem) where T : class {
			Marshal.WriteIntPtr(Mem + 4, typeof(T).TypeHandle.Value);
			T Obj = ToObject<T>(Mem + 4);
			return Obj;
		}

		public static T Allocate<T>(bool InvokeCTor = false) where T : class {
			int Size = SizeOf<T>();
			IntPtr Mem = Marshal.AllocHGlobal(Size);

			for (int i = 0; i < Size; i++)
				Marshal.WriteByte(Mem + i, 0);

			T Obj = Allocate<T>(Mem);
			if (InvokeCTor)
				InvokeConstructor(Obj);
			return Obj;
		}

		public static void InvokeConstructor<T>(T Obj) where T : class {
			ConstructorInfo TCtor = typeof(T).GetConstructor(new Type[] { });
			TCtor.Invoke(Obj, new object[] { });
		}

		public static void Free<T>(T Obj) where T : class {
			IntPtr Ptr = ToPointer(Obj);
			if (Ptr != IntPtr.Zero)
				Marshal.FreeHGlobal(Ptr - 4);
		}
	}
}